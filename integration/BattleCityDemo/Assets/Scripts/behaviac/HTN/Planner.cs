using System;
using System.Collections.Generic;

namespace behaviac
{
    public class Planner
    {
        #region Public delegates and events

        public delegate void PlannerCallback(Planner planner);

        public delegate void TaskCallback(Planner planner, PlannerTask task);

        #region Planning events

        /// <summary>
        /// This event is raised whenever the planner is attempting to generate a new task
        /// </summary>
        public event PlannerCallback PlanningStarted;

        /// <summary>
        /// This event is raised whenever a planning attempt has completed
        /// </summary>
        public event PlannerCallback PlanningEnded;

        /// <summary>
        /// This event is raised whenever a new task is generated
        /// </summary>
        public event TaskCallback PlanGenerated;

        /// <summary>
        /// This event is raised whenever a task is discarded due to being lower priority than the
        /// currently running task
        /// </summary>
        public event PlannerCallback PlanDiscarded;

        /// <summary>
        /// This event is raised whenever the planner fails to generate a valid task
        /// </summary>
        public event PlannerCallback PlanningFailed;

        #endregion Planning events

        #region Plan execution events

        /// <summary>
        /// This event is raised whenever a task starts executing
        /// </summary>
        public event TaskCallback PlanExecuted;

        /// <summary>
        /// This event is raised whenever a running task is aborted due to a new task being exeuted
        /// </summary>
        public event TaskCallback PlanAborted;

        /// <summary>
        /// This event is raised whenever a running task has completed successfully
        /// </summary>
        public event TaskCallback PlanCompleted;

        /// <summary>
        /// This event is raised whenever a running task has returned a failure status code
        /// </summary>
        public event TaskCallback PlanFailed;

        #endregion Plan execution events

        #region Task Execution events

        /// <summary>
        /// This event is raised whenever a new task has begun execution
        /// </summary>
        public event TaskCallback TaskExecuted;

        /// <summary>
        /// This event is raised whenever a task has successfully completed execution
        /// </summary>
        public event TaskCallback TaskSucceeded;

        /// <summary>
        /// This event is raised whenever a task fails during execution
        /// </summary>
        public event TaskCallback TaskFailed;

        #endregion Task Execution events

        #endregion Public delegates and events

        #region private fields

        /// <summary>
        /// Gets or sets the agent script instance that taskner will be generating plans for
        /// </summary>
        private Agent agent;

        public Agent GetAgent()
        {
            return this.agent;
        }
        /// <summary>
        /// Gets or sets whether the planner will automatically perform periodic replanning
        /// </summary>
        private bool AutoReplan = true;

        /// <summary>
        /// Gets or sets the amount of time between replanning attempts
        /// </summary>
        private float AutoReplanInterval = 0.2f;

        private Task m_rootTaskNode;
        private float timeTillReplan = 0f;

        #endregion private fields

        #region Public properties

        private PlannerTask m_rootTask;

        #endregion Public properties

        #region events

        public void Init(Agent pAgent, Task rootTask)
        {
            this.agent = pAgent;
            this.m_rootTaskNode = rootTask;
        }

        public void Uninit()
        {
            this.OnDisable();
        }

        private void OnDisable()
        {
            if (this.m_rootTask != null)
            {
                if (this.m_rootTask.GetStatus() == EBTStatus.BT_RUNNING)
                {
                    this.m_rootTask.abort(this.agent);
                    raisePlanAborted(this.m_rootTask);
                    BehaviorTask.DestroyTask(this.m_rootTask);
                }

                this.m_rootTask = null;
            }
        }

        public EBTStatus Update()
        {
            if (this.agent == null)
            {
                throw new InvalidOperationException("The Planner.Agent field has not been assigned");
            }

            doAutoPlanning();

            if (this.m_rootTask == null)
            {
                return EBTStatus.BT_FAILURE;
            }

            // Need a local reference in case the this.m_rootTask is cleared by an event handler
            var rootTask = this.m_rootTask;

            var taskStatus = rootTask.exec(this.agent);

            switch (taskStatus)
            {
                case EBTStatus.BT_RUNNING:
                    raiseTaskExecuted();
                    break;

                case EBTStatus.BT_SUCCESS:
                    raisePlanCompleted(rootTask);
                    break;

                case EBTStatus.BT_FAILURE:
                    raisePlanFailed(rootTask);
                    break;

                default:
                    throw new InvalidOperationException("Unhandled EBTStatus value: " + taskStatus);
            }

            return taskStatus;
        }

        #endregion events

        /// <summary>
        /// Generate a new task for the <paramref name="agent"/> based on the current world state as
        /// described by the <paramref name="agentState"/>.
        /// </summary>
        /// <param name="agent">
        /// The agent for which the task is being generated. This object instance must be of the
        /// same type as the type for which the Task was developed
        /// </param>
        /// <param name="agentState">The current world state required by the planner</param>
        /// <returns></returns>
        private PlannerTask GeneratePlan()
        {
            // If the planner is currently executing a task marked NotInterruptable, do not generate
            // any new plans.
            if (!canInterruptCurrentPlan())
            {
                raisePlanDiscarded();
                return null;
            }

            try
            {
                raisePlanningStarted();

                PlannerTask newPlan = this.BuildPlan(this.m_rootTaskNode);

                if (newPlan == null)
                {
                    raisePlanningFailed();
                    return null;
                }

                if (!newPlan.IsHigherPriority(this.m_rootTask))
                {
                    raisePlanDiscarded();
                    return null;
                }

                raisePlanGenerated(newPlan);

                return newPlan;
            }

            finally
            {
                raisePlanningEnded();
            }
        }

        #region Event wrappers

        private void raiseTaskSucceeded(PlannerTask task)
        {
            if (task != null && this.TaskSucceeded != null)
            {
                this.TaskSucceeded(this, task);
            }
        }

        private void raiseTaskExecuted()
        {
            if (this.TaskExecuted != null)
            {
                this.TaskExecuted(this, this.m_rootTask);
            }
        }

        private void raisePlanningStarted()
        {
            if (this.PlanningStarted != null)
            {
                this.PlanningStarted(this);
            }
        }

        private void raisePlanningEnded()
        {
            if (this.PlanningEnded != null)
            {
                this.PlanningEnded(this);
            }
        }

        private void raisePlanCompleted(PlannerTask task)
        {
            if (this.PlanCompleted != null)
            {
                this.PlanCompleted(this, task);
            }
        }

        private void raisePlanFailed(PlannerTask task)
        {
            if (this.TaskFailed != null)
            {
                this.TaskFailed(this, task);
            }

            if (this.PlanFailed != null)
            {
                this.PlanFailed(this, task);
            }
        }

        private void raisePlanGenerated(PlannerTask task)
        {
            if (this.PlanGenerated != null)
            {
                this.PlanGenerated(this, task);
            }
        }

        private void raisePlanDiscarded()
        {
            if (this.PlanDiscarded != null)
            {
                this.PlanDiscarded(this);
            }
        }

        private void raisePlanExecuted(PlannerTask task)
        {
            if (this.PlanExecuted != null)
            {
                this.PlanExecuted(this, task);
            }
        }

        private void raisePlanAborted(PlannerTask task)
        {
            if (this.PlanAborted != null)
            {
                this.PlanAborted(this, task);
            }
        }

        private void raisePlanningFailed()
        {
            if (this.PlanningFailed != null)
            {
                this.PlanningFailed(this);
            }
        }

        #endregion Event wrappers

        #region Private utility methods

        private bool canInterruptCurrentPlan()
        {
            if (this.m_rootTask == null)
            {
                return true;
            }

            if (this.m_rootTask.GetStatus() != EBTStatus.BT_RUNNING)
            {
                return true;
            }

            var task = this.m_rootTask;

            if (task == null || !task.NotInterruptable)
            {
                return true;
            }

            return task.GetStatus() == EBTStatus.BT_FAILURE || task.GetStatus() == EBTStatus.BT_SUCCESS;
        }

        private void doAutoPlanning()
        {
            if (!this.AutoReplan)
            {
                return;
            }

            this.timeTillReplan -= Workspace.Instance.DeltaTime;

            var noPlan = this.m_rootTask == null || this.m_rootTask.GetStatus() != EBTStatus.BT_RUNNING;

            //if (noPlan || timeTillReplan <= 0)
            if (noPlan)
            {
                timeTillReplan += AutoReplanInterval;

                PlannerTask newPlan = this.GeneratePlan();

                if (newPlan != null)
                {
                    if (this.m_rootTask != null)
                    {
                        if (this.m_rootTask.GetStatus() == EBTStatus.BT_RUNNING)
                        {
                            this.m_rootTask.abort(this.agent);
                            raisePlanAborted(this.m_rootTask);
                        }

                        BehaviorTask.DestroyTask(this.m_rootTask);
                    }

                    this.m_rootTask = newPlan;
                }
            }
        }

        #endregion Private utility methods

        #region Log

        private void LogPlanBegin(Agent a, Task root)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string agentClassName = a.GetClassTypeName();
                string agentInstanceName = a.GetName();

                agentClassName = agentClassName.Replace(".", "::");
                agentInstanceName = agentInstanceName.Replace(".", "::");

                string ni = BehaviorTask.GetTickInfo(a, root, "plan");
                int count = Workspace.Instance.GetActionCount(ni) + 1;
                string buffer = string.Format("[plan_begin]{0}#{1} {2} {3}\n", agentClassName, agentInstanceName, ni, count);

                LogManager.Log(buffer);

                a.Variables.Log(a, true);
            }

#endif
        }

        private void LogPlanEnd(Agent a, Task root)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string agentClassName = a.GetClassTypeName();
                string agentInstanceName = a.GetName();

                agentClassName = agentClassName.Replace(".", "::");
                agentInstanceName = agentInstanceName.Replace(".", "::");

                string ni = BehaviorTask.GetTickInfo(a, root, null);
                string buffer = string.Format("[plan_end]{0}#{1} {2}\n", agentClassName, agentInstanceName, ni);

                LogManager.Log(buffer);
            }

#endif
        }

        private void LogPlanNodeBegin(Agent a, BehaviorNode n)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, n, null);

                LogManager.Log("[plan_node_begin]{0}\n", ni);
                a.Variables.Log(a, true);
            }

#endif
        }

        private void LogPlanNodePreconditionFailed(Agent a, BehaviorNode n)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, n, null);

                LogManager.Log("[plan_node_pre_failed]{0}\n", ni);
            }

#endif
        }

        private void LogPlanNodeEnd(Agent a, BehaviorNode n, string result)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, n, null);

                LogManager.Log("[plan_node_end]{0} {1}\n", ni, result);
            }

#endif
        }

        public void LogPlanReferenceTreeEnter(Agent a, ReferencedBehavior referencedNode)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, referencedNode, null);
                LogManager.Log("[plan_referencetree_enter]{0} {1}.xml\n", ni, referencedNode.ReferencedTree);
            }

#endif
        }

        public void LogPlanReferenceTreeExit(Agent a, ReferencedBehavior referencedNode)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, referencedNode, null);
                LogManager.Log("[plan_referencetree_exit]{0} {1}.xml\n", ni, referencedNode.ReferencedTree);
            }

#endif
        }

        private void LogPlanMethodBegin(Agent a, BehaviorNode m)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, m, null);
                LogManager.Log("[plan_method_begin]{0}\n", ni);

                a.Variables.Log(a, true);
            }

#endif
        }

        private void LogPlanMethodEnd(Agent a, BehaviorNode m, string result)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, m, null);
                LogManager.Log("[plan_method_end]{0} {1}\n", ni, result);
            }

#endif
        }

        public void LogPlanForEachBegin(Agent a, DecoratorIterator pForEach, int index, int count)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, pForEach, null);
                LogManager.Log("[plan_foreach_begin]{0} {1} {2}\n", ni, index, count);
                a.Variables.Log(a, true);
            }

#endif
        }

        public void LogPlanForEachEnd(Agent a, DecoratorIterator pForEach, int index, int count, string result)
        {
#if !BEHAVIAC_RELEASE

            if (Config.IsLoggingOrSocketing)
            {
                string ni = BehaviorTask.GetTickInfo(a, pForEach, null);
                LogManager.Log("[plan_foreach_end]{0} {1} {2} {3}\n", ni, index, count, result);
            }

#endif
        }

        #endregion Log

        #region Plan Builder

        private PlannerTask BuildPlan(Task root)
        {
            LogManager.PLanningClearCache();

            int depth = this.agent.Variables.Depth;

            PlannerTask rootTask = null;
            using(var currentState = this.agent.Variables.Push(true))
            {
                this.agent.PlanningTop = this.agent.Variables.Top;
                Debug.Check(this.agent.PlanningTop >= 0);

                LogPlanBegin(this.agent, root);

                rootTask = this.decomposeNode(root, 0);

                LogPlanEnd(this.agent, root);

#if !BEHAVIAC_RELEASE
                BehaviorTask.CHECK_BREAKPOINT(this.agent, root, "plan", EActionResult.EAR_all);
#endif

                this.agent.PlanningTop = -1;
            }

            Debug.Check(this.agent.Variables.Depth == depth);

            return rootTask;
        }

        #endregion Plan Builder

        #region Private decompose methods

        public PlannerTask decomposeNode(BehaviorNode node, int depth)
        {
            try
            {
                // Ensure that the planner does not get stuck in an infinite loop
                if (depth >= 256)
                {
                    Debug.LogError("Exceeded task nesting depth. Does the graph contain an invalid cycle?");
                    return null;
                }

                LogPlanNodeBegin(this.agent, node);

                int depth1 = this.agent.Variables.Depth;
                PlannerTask taskAdded = null;

                bool isPreconditionOk = node.CheckPreconditions(this.agent, false);

                if (isPreconditionOk)
                {
                    bool bOk = true;
                    taskAdded = PlannerTask.Create(node, this.agent);

                    if (node is Action)
                    {
                        //nothing to do for action
                        Debug.Check(true);

                    }
                    else
                    {
                        Debug.Check(taskAdded is PlannerTaskComplex);
                        PlannerTaskComplex seqTask = taskAdded as PlannerTaskComplex;

                        bOk = this.decomposeComplex(node, seqTask, depth);
                    }

                    if (bOk)
                    {
                        node.ApplyEffects(this.agent, Effector.EPhase.E_SUCCESS);

                    }
                    else
                    {
                        BehaviorTask.DestroyTask(taskAdded);
                        taskAdded = null;
                    }

                }
                else
                {
                    //precondition failed
                    LogPlanNodePreconditionFailed(this.agent, node);
                }

                LogPlanNodeEnd(this.agent, node, taskAdded != null ? "success" : "failure");

                Debug.Check(this.agent.Variables.Depth == depth1);

                return taskAdded;

            }
            catch (Exception ex)
            {
                Debug.Check(false, ex.Message);
            }

            return null;
        }

        private bool decomposeComplex(BehaviorNode node, PlannerTaskComplex seqTask, int depth)
        {
            try
            {

                int depth1 = this.agent.Variables.Depth;
                bool bOk = false;
                bOk = node.decompose(node, seqTask, depth, this);

                Debug.Check(this.agent.Variables.Depth == depth1);
                return bOk;

            }
            catch (Exception ex)
            {
                Debug.Check(false, ex.Message);
            }

            return false;
        }

        //private PlannerTask decomposeTask(Task task, int depth) {
        //because called form other node , so change the private to public
        public PlannerTask decomposeTask(Task task, int depth)
        {
            var methodsCount = task.GetChildrenCount();

            if (methodsCount == 0)
            {
                return null;
            }

            int depth1 = this.agent.Variables.Depth;
            PlannerTask methodTask = null;

            for (int i = 0; i < methodsCount; i++)
            {
                BehaviorNode method = task.GetChild(i);
                Debug.Check(method is Method);
                int depth2 = this.agent.Variables.Depth;
                using(var currentState = this.agent.Variables.Push(false))
                {
                    LogPlanMethodBegin(this.agent, method);
                    methodTask = this.decomposeNode(method, depth + 1);
                    LogPlanMethodEnd(this.agent, method, methodTask != null ? "success" : "failure");

                    if (methodTask != null)
                    {
                        // succeeded
                        break;
                    }
                }

                Debug.Check(this.agent.Variables.Depth == depth2);
            }

            Debug.Check(this.agent.Variables.Depth == depth1);
            return methodTask;
        }

        #endregion Private decompose methods
    }
}
