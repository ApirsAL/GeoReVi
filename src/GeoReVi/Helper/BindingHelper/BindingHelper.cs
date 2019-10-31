using Caliburn.Micro;
using System.Linq;

namespace GeoReVi
{
    public static class BindingHelper
    {
        /// <summary>
        /// Enabling nested binding for the caliburn framework
        /// </summary>
        public static void EnableNestedViewModelActionBinding()
        {
            var baseGetTargetMethod = ActionMessage.GetTargetMethod;
            ActionMessage.GetTargetMethod = (message, target) =>
            {
                var methodName = GetRealMethodName(message.MethodName, ref target);
                if (methodName == null)
                    return null;

                var fakeMessage = new ActionMessage { MethodName = methodName };
                foreach (var p in message.Parameters)
                    fakeMessage.Parameters.Add(p);
                return baseGetTargetMethod(fakeMessage, target);
            };

            var baseSetMethodBinding = ActionMessage.SetMethodBinding;
            ActionMessage.SetMethodBinding = context =>
            {
                baseSetMethodBinding(context);
                var target = context.Target;
                if (target != null)
                {
                    GetRealMethodName(context.Message.MethodName, ref target);
                    context.Target = target;
                }
            };
        }

        private static string GetRealMethodName(string methodName, ref object target)
        {
            var parts = methodName.Split('.');
            var model = target;
            foreach (var propName in parts.Take(parts.Length - 1))
            {
                if (model == null)
                    return null;

                var prop = model.GetType().GetPropertyCaseInsensitive(propName);
                if (prop == null || !prop.CanRead)
                    return null;

                model = prop.GetValue(model);
            }
            target = model;
            return parts.Last();
        }
    }
}
