using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PDNUtils.Runner.Attributes;

namespace PDNUtils.Runner
{
    /// <summary>
    /// Small class with attribute-driven method for executing methods from arbitrary class.
    /// Method should be static and has to be marked with attribute Run to be executed.
    /// </summary>
    public class Runner
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Action<string> beforeInvoke;

        private readonly MessageHandler messageHandler;

        public Runner()
        {
        }

        public Runner(Action<string> beforeInvoke, MessageHandler messageHandler)
        {
            if (beforeInvoke == null) { throw new ArgumentNullException("beforeInvoke"); }
            if (messageHandler == null) { throw new ArgumentNullException("messageHandler"); }

            this.beforeInvoke = beforeInvoke;
            this.messageHandler = messageHandler;
        }

        public void ExecuteMethods()
        {
            try
            {
                LoadAssembliesInternal();
                ExecuteTypeMethodsInternal();
            }
            catch (Exception e)
            {
                var extendedexceptionDetails = PDNUtils.Help.Utils.GetExtendedExceptionDetails(e);
                log.ErrorFormat("Something failed: {0}", extendedexceptionDetails);
                if (messageHandler != null)
                {
                    messageHandler.Error(extendedexceptionDetails);
                }
            }
        }

        private void LoadAssembliesInternal()
        {
            var path =
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var files = Directory.GetFiles(path, "*.dll");

            foreach (var file in files)
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                var a = Assembly.Load(fileNameWithoutExtension);
                log.InfoFormat("loaded '{0}'", a.GetName());
            }
        }

        private void ExecuteTypeMethodsInternal()
        {
            var typeGroups = (from assembly in System.AppDomain.CurrentDomain.GetAssemblies()
                              let types = (
                                  from c in GetTypesSafe(assembly)
                                  where
                                      (c.GetCustomAttributes(true).Count(attr => attr is RunableClass) != 0 || typeof(RunableBase).IsAssignableFrom(c))
                                      &&
                                      c.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                                          .Count(
                                              method =>
                                                  method.GetCustomAttributes(true)
                                                      .Count(
                                                          attr =>
                                                              attr is RunAttribute &&
                                                              ((RunAttribute)attr).Enabled
                                                      ) != 0
                                          ) != 0
                                  select c
                                  )
                              where types.Count() > 0
                              group types by assembly.GetName().Name
                );

            foreach (var typeGroup in typeGroups)
            {
                foreach (var enumerable in typeGroup)
                {
                    foreach (var type in enumerable)
                    {
                        ExecuteTypeMethodsInternal(type);
                    }
                }
            }

        }

        private static Type[] GetTypesSafe(Assembly a)
        {
            try
            {
                return a.GetTypes();
            }
            catch (Exception e)
            {
                log.ErrorFormat("Failed to load types from assembly {0}", a.FullName);
                log.Error(e);

                if (e is ReflectionTypeLoadException)
                {
                    var reflectionTypeLoadException = (e as ReflectionTypeLoadException);
                    var details = string.Join(Environment.NewLine, reflectionTypeLoadException.LoaderExceptions.Select(ex => Help.Utils.GetExtendedExceptionDetails(ex)));
                    log.ErrorFormat("Details: {0}", details);
                    var types = reflectionTypeLoadException.Types.Where(t => t != null).ToArray();
                    return types;
                }
            }

            return new Type[] { };
        }

        private void ExecuteTypeMethodsInternal(Type type)
        {
            log.InfoFormat("start executing {0}", type.Name);

            //bool isStaticClass = type.IsAbstract && type.IsSealed;// there is no IsStatic property, but static classes are sealed and it' impossible to instantiate them so they are abstract

            //var instance = isStaticClass ? null : Activator.CreateInstance(type);// don't even try to create instance of static class

            object instance = null;

            try { instance = GetInstanceInternal(type); }
            catch { }

            IEnumerable<MethodInfo> runnableMethods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                                                    BindingFlags.Static | BindingFlags.Instance).Where(
                                                                        method =>
                                                                        method.GetCustomAttributes(true).Where(
                                                                            attr =>
                                                                            attr is RunAttribute &&
                                                                            ((RunAttribute)attr).Enabled).Count() != 0
                );

            foreach (MethodInfo method in runnableMethods)
            {
                string name = method.Name;

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("started executing {0}", name);
                }

                try
                {
                    if (beforeInvoke != null)
                    {
                        beforeInvoke(string.Format("[{0}]::{1}", type.FullName, name));
                    }
                    method.Invoke(instance, BindingFlags.Static, null, null, null);
                }
                catch (Exception e)
                {
                    CallOnError(e);
                    log.Error(e);
                }

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("finished executing {0}", name);
                }
            }

            log.Info("finished executing");
        }

        private object GetInstanceInternal(Type type)
        {
            bool isStaticClass = type.IsAbstract && type.IsSealed;// there is no IsStatic property, but static classes are sealed and it' impossible to instantiate them so they are abstract

            if (!isStaticClass && beforeInvoke != null)
            {
                beforeInvoke(string.Format("[{0}]::ctor", type.FullName));
            }

            if (messageHandler != null)
            {
                ConstructorInfo ci = type.GetConstructor(new[] { typeof(MessageHandler) });

                if (ci != null)
                {
                    return ci.Invoke(new object[] { messageHandler });
                }
            }

            var instance = isStaticClass ? null : GetObjectWithMessageHandlerProperty(type);// don't even try to create instance of static class

            return instance;
        }

        private object GetObjectWithMessageHandlerProperty(Type type)
        {
            var instance = Activator.CreateInstance(type);

            var propertyInfo = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty).FirstOrDefault(p => p.PropertyType == typeof(MessageHandler));

            if (propertyInfo != null && propertyInfo.DeclaringType != null)
            {
                propertyInfo = propertyInfo.DeclaringType.GetProperty(propertyInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty);
                propertyInfo.SetValue(instance, messageHandler, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty, null, null, null);
            }

            return instance;
        }

        public event Action<Exception> OnError;

        private void CallOnError(Exception e)
        {
            Action<Exception> handler = OnError;

            if (handler != null)
            {
                if (e is TargetInvocationException)
                {
                    handler(e.InnerException);
                }
                else
                {
                    handler(e);
                }
            }
        }
    }
}