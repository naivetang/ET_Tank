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
            FieldInfo field;
            Type[] args;
            Type type = typeof(ETModel.TurretComponent);

            field = type.GetField("UpdatePos", flag);
            app.RegisterCLRFieldGetter(field, get_UpdatePos_0);
            app.RegisterCLRFieldSetter(field, set_UpdatePos_0);


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
