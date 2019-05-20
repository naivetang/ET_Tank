using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class ETModel_TurretComponent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.TurretComponent);
            args = new Type[]{typeof(System.Single), typeof(System.Single)};
            method = type.GetMethod("NetUpdate", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, NetUpdate_0);

            field = type.GetField("UpdatePos", flag);
            app.RegisterCLRFieldGetter(field, get_UpdatePos_0);
            app.RegisterCLRFieldSetter(field, set_UpdatePos_0);


        }


        static StackObject* NetUpdate_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @RY = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @RX = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            ETModel.TurretComponent instance_of_this_method = (ETModel.TurretComponent)typeof(ETModel.TurretComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.NetUpdate(@RX, @RY);

            return __ret;
        }


        static object get_UpdatePos_0(ref object o)
        {
            return ETModel.TurretComponent.UpdatePos;
        }
        static void set_UpdatePos_0(ref object o, object v)
        {
            ETModel.TurretComponent.UpdatePos = (System.Action<System.Single, System.Single>)v;
        }


    }
}
