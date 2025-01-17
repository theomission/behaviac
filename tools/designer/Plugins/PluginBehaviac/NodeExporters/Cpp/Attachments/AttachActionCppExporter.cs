/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Behaviac.Design;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attachments;
using PluginBehaviac.DataExporters;

namespace PluginBehaviac.NodeExporters
{
    public class AttachActionCppExporter : AttachmentCppExporter
    {
        protected override bool ShouldGenerateClass()
        {
            return true;
        }

        protected override void GenerateConstructor(Attachment attachment, StreamWriter stream, string indent, string className)
        {
            base.GenerateConstructor(attachment, stream, indent, className);

            AttachAction attach = attachment as AttachAction;
            if (attach == null)
                return;

            RightValueCppExporter.GenerateClassConstructor(attach.Opl, stream, indent, "opl");

            if (!attach.IsAction())
            {
                if (attach.IsCompute() && attach.Opr1 != null)
                {
                    RightValueCppExporter.GenerateClassConstructor(attach.Opr1, stream, indent, "opr1");
                }

                if (attach.Opr2 != null)
                {
                    RightValueCppExporter.GenerateClassConstructor(attach.Opr2, stream, indent, "opr2");
                }
            }
        }

        protected override void GenerateMember(Attachment attachment, StreamWriter stream, string indent)
        {
            base.GenerateMember(attachment, stream, indent);

            AttachAction attach = attachment as AttachAction;
            if (attach == null)
                return;

            RightValueCppExporter.GenerateClassMember(attach.Opl, stream, indent, "opl");

            if (!attach.IsAction())
            {
                if (attach.IsCompute() && attach.Opr1 != null)
                {
                    RightValueCppExporter.GenerateClassMember(attach.Opr1, stream, indent, "opr1");
                }

                if (attach.Opr2 != null)
                {
                    RightValueCppExporter.GenerateClassMember(attach.Opr2, stream, indent, "opr2");
                }
            }
        }

        protected override void GenerateMethod(Attachment attachment, StreamWriter stream, string indent)
        {
            base.GenerateMethod(attachment, stream, indent);

            AttachAction attach = attachment as AttachAction;
            if (attach == null)
                return;

            stream.WriteLine("{0}\t\tvirtual EBTStatus update_impl(Agent* pAgent, EBTStatus childStatus)", indent);
            stream.WriteLine("{0}\t\t{{", indent);
            stream.WriteLine("{0}\t\t\tEBTStatus result = BT_SUCCESS;", indent);

            if (attach.IsAction())
            {
                string method = MethodCppExporter.GenerateCode(attach.Opl.Method, stream, indent + "\t\t\t", string.Empty, string.Empty, "opl");

                stream.WriteLine("{0}\t\t\t{1};", indent, method);
                MethodCppExporter.PostGenerateCode(attach.Opl.Method, stream, indent + "\t\t\t", string.Empty, string.Empty, "opl");
            }
            else if (attach.IsAssign())
            {
                if (attach.Opl != null && !attach.Opl.IsMethod && attach.Opl.Var != null && attach.Opr2 != null)
                {
                    PropertyDef prop = attach.Opl.Var.Property;
                    if (prop != null)
                    {
                        RightValueCppExporter.GenerateCode(attach.Opr2, stream, indent + "\t\t\t", attach.Opr2.NativeType, "opr2", "opr2");

                        string property = PropertyCppExporter.GetProperty(prop, attach.Opl.Var.ArrayIndexElement, stream, indent + "\t\t\t", "opl", "attach");
                        string propName = prop.BasicName.Replace("[]", "");

                        if (prop.IsArrayElement && attach.Opl.Var.ArrayIndexElement != null)
                        {
                            ParameterCppExporter.GenerateCode(attach.Opl.Var.ArrayIndexElement, stream, indent + "\t\t\t", "int", "opl_index", "attach_opl");
                            property = string.Format("({0})[opl_index]", property);
                        }

                        string propBasicName = prop.BasicName.Replace("[]", "");
                        if (!prop.IsArrayElement && (prop.IsPar || prop.IsCustomized))
                        {
                            uint id = Behaviac.Design.CRC32.CalcCRC(propBasicName);

                            stream.WriteLine("{0}\t\t\tBEHAVIAC_ASSERT(behaviac::MakeVariableId(\"{1}\") == {2}u);", indent, propBasicName, id);
                            stream.WriteLine("{0}\t\t\tpAgent->SetVariable(\"{1}\", opr2, {2}u);", indent, propBasicName, id);
                        }
                        else
                        {
                            stream.WriteLine("{0}\t\t\t{1} = opr2;", indent, property);
                        }

                        if (attach.Opr2.IsMethod)
                        {
                            RightValueCppExporter.PostGenerateCode(attach.Opr2, stream, indent + "\t\t\t", attach.Opr2.NativeType, "opr2", string.Empty);
                        }
                    }
                }
            }
            else if (attach.IsCompare())
            {
                string typeName = DataCppExporter.GetGeneratedNativeType(attach.Opl.ValueType);

                RightValueCppExporter.GenerateCode(attach.Opl, stream, indent + "\t\t\t", typeName, "opl", "");
                RightValueCppExporter.GenerateCode(attach.Opr2, stream, indent + "\t\t\t", typeName, "opr2", "");

                if (attach.Opl != null && attach.Opl.IsMethod)
                {
                    RightValueCppExporter.PostGenerateCode(attach.Opl, stream, indent + "\t\t\t", typeName, "opl", "");
                }

                if (attach.Opr2 != null && attach.Opr2.IsMethod)
                {
                    RightValueCppExporter.PostGenerateCode(attach.Opr2, stream, indent + "\t\t\t", typeName, "opr2", "");
                }

                switch (attach.Operator)
                {
                    case OperatorTypes.Equal:
                        stream.WriteLine("{0}\t\t\tbool op = Details::Equal(opl, opr2);", indent);
                        break;

                    case OperatorTypes.NotEqual:
                        stream.WriteLine("{0}\t\t\tbool op = !Details::Equal(opl, opr2);", indent);
                        break;

                    case OperatorTypes.Greater:
                        stream.WriteLine("{0}\t\t\tbool op = Details::Greater(opl, opr2);", indent);
                        break;

                    case OperatorTypes.GreaterEqual:
                        stream.WriteLine("{0}\t\t\tbool op = Details::GreaterEqual(opl, opr2);", indent);
                        break;

                    case OperatorTypes.Less:
                        stream.WriteLine("{0}\t\t\tbool op = Details::Less(opl, opr2);", indent);
                        break;

                    case OperatorTypes.LessEqual:
                        stream.WriteLine("{0}\t\t\tbool op = Details::LessEqual(opl, opr2);", indent);
                        break;

                    default:
                        stream.WriteLine("{0}\t\t\tbool op = false;", indent);
                        break;
                }

                stream.WriteLine("{0}\t\t\tif (!op)", indent);
                stream.WriteLine("{0}\t\t\t\tresult = BT_FAILURE;", indent);
            }
            else if (attach.IsCompute())
            {
                if (attach.Opl != null && !attach.Opl.IsMethod && attach.Opl.Var != null && attach.Opr1 != null && attach.Opr2 != null)
                {
                    PropertyDef prop = attach.Opl.Var.Property;
                    if (prop != null)
                    {
                        string typeName = DataCppExporter.GetGeneratedNativeType(attach.Opr1.ValueType);

                        RightValueCppExporter.GenerateCode(attach.Opr1, stream, indent + "\t\t\t", typeName, "opr1", "opr1");
                        RightValueCppExporter.GenerateCode(attach.Opr2, stream, indent + "\t\t\t", typeName, "opr2", "opr2");

                        string oprStr = string.Empty;
                        switch (attach.Operator)
                        {
                            case OperatorTypes.Add:
                                oprStr = "opr1 + opr2";
                                break;

                            case OperatorTypes.Sub:
                                oprStr = "opr1 - opr2";
                                break;

                            case OperatorTypes.Mul:
                                oprStr = "opr1 * opr2";
                                break;

                            case OperatorTypes.Div:
                                oprStr = "opr1 / opr2";
                                break;

                            default:
                                Debug.Check(false, "The operator is wrong!");
                                break;
                        }

                        oprStr = string.Format("({0})({1})", typeName, oprStr);

                        string property = PropertyCppExporter.GetProperty(prop, attach.Opl.Var.ArrayIndexElement, stream, indent + "\t\t\t", "opl", "attach");
                        string propName = prop.BasicName.Replace("[]", "");

                        if (prop.IsArrayElement && attach.Opl.Var.ArrayIndexElement != null)
                        {
                            ParameterCppExporter.GenerateCode(attach.Opl.Var.ArrayIndexElement, stream, indent + "\t\t\t", "int", "opl_index", "attach_opl");
                            property = string.Format("({0})[opl_index]", property);
                        }

                        string propBasicName = prop.BasicName.Replace("[]", "");
                        if (!prop.IsArrayElement && (prop.IsPar || prop.IsCustomized))
                        {
                            uint id = Behaviac.Design.CRC32.CalcCRC(propBasicName);

                            stream.WriteLine("{0}\t\t\tBEHAVIAC_ASSERT(behaviac::MakeVariableId(\"{1}\") == {2}u);", indent, propBasicName, id);
                            stream.WriteLine("{0}\t\t\tpAgent->SetVariable(\"{1}\", {2}, {3}u);", indent, propBasicName, oprStr, id);
                        }
                        else
                        {
                            stream.WriteLine("{0}\t\t\t{1} = {2};", indent, property, oprStr);
                        }

                        if (attach.Opr1.IsMethod)
                        {
                            RightValueCppExporter.PostGenerateCode(attach.Opr1, stream, indent + "\t\t\t", typeName, "opr1", string.Empty);
                        }

                        if (attach.Opr2.IsMethod)
                        {
                            RightValueCppExporter.PostGenerateCode(attach.Opr2, stream, indent + "\t\t\t", typeName, "opr2", string.Empty);
                        }
                    }
                }
            }

            stream.WriteLine("{0}\t\t\treturn result;", indent);
            stream.WriteLine("{0}\t\t}}", indent);
        }
    }
}
