using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Injector;
using ThinkCrm.Core.Interfaces;
using ThinkCrm.Core.PluginCore.Attributes;
using ThinkCrm.Core.PluginCore.Helper;
using ThinkCrm.Core.PluginCore.Logging;
using ThinkCrm.Core.PluginCore.Validator;

namespace ThinkCrm.Core.PluginCore
{
    public abstract class CorePlugin : IPlugin
    {

        protected readonly string ClassName;
        private ValidationBuilder _validationBuilder;
        private bool _useAttributesForValidation;
        protected readonly IInjectorService ObjectProviderService;

        protected CorePlugin() : this(new InjectorService())
        {            
            
        }

        protected CorePlugin(IInjectorService injectorService)
        {
            if (injectorService == null) throw new ArgumentNullException(nameof(injectorService));
            ClassName = GetType().FullName;
            ObjectProviderService = injectorService;
        }

        public void Execute(IServiceProvider serviceProvider)
        {            
            try
            {
                var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                tracing.Trace("Started Execute");
                var p = new PluginSetup(serviceProvider, ClassName);                
                CoreExecute(p);
            }
            catch (InvalidPluginExecutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                tracing.Trace($"CorePlugin.Execute|Unhandled exception received. Message: {ex.Message}\nStack Trace: {ex.StackTrace}\nInner Exception: {ex.InnerException?.Message ?? "(None)"}\nInner Execption Stack Trace: {ex.InnerException?.StackTrace ?? "(None)"}");
                throw new InvalidPluginExecutionException(PluginConstants.UserErrorMessage, ex);
            }

        }

        private void CoreExecute(IPluginSetup pluginSetup)
        {
            var p = pluginSetup;
            
            var l = new LocalLogger(p.Logging, ClassName);
            
            l.Write("PluginSetup Completed.");
            
            var sw = new Stopwatch();
            sw.Start();

            try
            {
                l.Write(PluginConstants.LogWithTime, "Plugin Validation Starting", sw.ElapsedMilliseconds);
                if (!ValidateExecution(p))
                {
                    l.Write(PluginConstants.LogWithTime, "Plugin Validation Returned False, terminating execution.", sw.ElapsedMilliseconds);
                    return;
                }
                l.Write(PluginConstants.LogWithTime, "Plugin Validation Returned True, continuing execution", sw.ElapsedMilliseconds);

                l.Write(PluginConstants.LogWithTime, "Plugin Execution Starting", sw.ElapsedMilliseconds);

                ExecutePluginCore(p);

                l.Write(PluginConstants.LogWithTime, "Plugin Execution Completed - No Exceptions",
                    sw.ElapsedMilliseconds);
            }
            catch (InvalidPluginExecutionException ex)
            {
                l.Write(PluginConstants.LogWithTime, "InvalidPluginExecutionException Caught", sw.ElapsedMilliseconds);
                l.Write(ex);
                throw;
            }
            catch (Exception ex)
            {
                l.Write(PluginConstants.LogWithTime, "Unhandled Exception Caught", sw.ElapsedMilliseconds);
                l.Write(ex);

                throw new InvalidPluginExecutionException(
                    PluginConstants.UserErrorMessage, ex);
            }
            finally
            {
                sw.Stop();
                l.Write(PluginConstants.LogWithTime, "Terminating Plugin Execution", sw.ElapsedMilliseconds);                                
            }
        }

        private void ExecutePluginCore(IPluginSetup p)
        {
            var continueExecute = false;
            var localLogger = new LocalLogger(p.Logging, ClassName);

            switch (p.Context.GetMode())
            {
                case ExecutionMode.Synchronous:
                    switch (p.Context.GetPipelineStage())
                    {
                        case PipelineStage.Prevalidation:
                            continueExecute = ExecutePreVal(p);
                            localLogger.Write($"ExecutePreVal Returned: {continueExecute}");                            
                            break;
                        case PipelineStage.Preoperation:
                            continueExecute = ExecutePreOp(p);
                            localLogger.Write($"ExecutePreOp Returned: {continueExecute}");
                            break;
                        case PipelineStage.Postoperation:
                            continueExecute = ExecutePostOpSync(p);
                            localLogger.Write($"ExecutePostOpSync Returned: {continueExecute}");
                            break;
                        default:
                            localLogger.Write("Failed to determine Execution Stage.");
                            break;
                    }
                    break;
                case ExecutionMode.Asynchronous:
                    continueExecute = ExecutePostOpAsync(p);
                    localLogger.Write($"ExecutePostOpAsync Returned: {continueExecute}");
                    break;
                default:
                    localLogger.Write("Failed to determine Execution Mode.");
                    break;
            }

            localLogger.Write($"continueExecute={continueExecute}");

            if (continueExecute)
            {
                localLogger.Write("Calling Execute method");
                Execute(p);
            }              
        }

        protected virtual bool Execute(IPluginSetup p)
        {
            return p.Logging.WithCaller(ClassName).WriteAndReturn(true, "Virtual Method Executed");
        }

        protected virtual bool ExecutePreVal(IPluginSetup p)
        {
            return p.Logging.WithCaller(ClassName).WriteAndReturn(true, "Virtual Method Executed");
        }

        protected virtual bool ExecutePreOp(IPluginSetup p)
        {
            return p.Logging.WithCaller(ClassName).WriteAndReturn(true, "Virtual Method Executed");
        }

        protected virtual bool ExecutePostOpSync(IPluginSetup p)
        {
            return p.Logging.WithCaller(ClassName).WriteAndReturn(true, "Virtual Method Executed");
        }

        protected virtual bool ExecutePostOpAsync(IPluginSetup p)
        {
            return p.Logging.WithCaller(ClassName).WriteAndReturn(true, "Virtual Method Executed");
        }

        /// <summary>
        /// Override to configure validator objects to be used by CorePlugin for plugin exection validation. This requires 
        /// more coding but can be more performant than using attributes. If you want to fully override CorePlugin's validation
        /// logic then override <see cref="ValidateExecution">ValidateExecution</see>.
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void ConfigurePluginValidation(IValidationBuilder builder)
        {
            return;            
        }

        /// <summary>
        /// Override to implement custom validation logic.        
        /// </summary>
        /// <param name="p"></param>
        /// <returns>True if Plugin Execution should continue. False if Plugin Execution should terminate without error.</returns>
        protected virtual bool ValidateExecution(IPluginSetup p)
        {            
            return p.Logging.WithCaller(ClassName).WriteAndReturn(InternalValidateExecution(p),
                "Virtual Method Executed resutling in call to InternalValidateExecution.");
        }

        /// <summary>
        /// Override and return false to prevent <see cref="ValidateExecution">ValidateExecution</see> from using attributes
        /// for validating execution. Override <see cref="ConfigurePluginValidation"/> to configure <see cref="IPluginValidator"/> objects 
        /// to be used by CorePlugin for validation.
        /// </summary>
        protected virtual bool UseAttributesForValidation => true;

        private bool InternalValidateExecution(IPluginSetup p)
        {
            var l = new LocalLogger(p.Logging, ClassName);

            l.Write("Starting");

            #region ValidationConfiguration

            if (_validationBuilder == null)
            {
                l.Write("Initial Configuration of Validation Logic.");

                _validationBuilder = new ValidationBuilder();
                _useAttributesForValidation = UseAttributesForValidation;

                l.Write("Executiing ConfigurePluginValidation.");
                ConfigurePluginValidation(_validationBuilder);

                if (_useAttributesForValidation)
                {
                    l.Write("Loading IPluginValidator Attributes.");
                    var attrs = Attribute.GetCustomAttributes(this.GetType());

                    foreach (var attr in attrs.Where(x => x is IPluginValidator))
                    {
                        l.Write($"Adding Attribute: {attr.GetType().FullName}");
                        _validationBuilder.Add((IPluginValidator)attr);
                    }
                }
            }

            #endregion

            var validators = _validationBuilder.GetAll;

            if (!validators.Any())
            {
                return l.WriteAndReturn(true, "No IPluginValidator objects configured. Returning true.");
            }

            if (validators.Any(x => x is SkipPluginRegistrationValidationAttribute))
            {
                return l.WriteAndReturn(true, "Found SkipPluginRegistrationValidation Attribute, Skipping Validation. Returning true.");
            }

            foreach (var attr in validators)
            {
                string errorMessage;
                var throwException = false;

                if (!attr.Validate(p.Context, out throwException, out errorMessage))
                {
                    l.Write($"Validation Failed|{errorMessage}|Throw Exception: {throwException}");
                    if (throwException) throw new InvalidPluginExecutionException(PluginConstants.UserErrorMessage);
                    return false;
                }
                else
                {
                    l.Write($"Validation Passed: {attr}");
                }
            }

            return l.WriteAndReturn(true, "Validation Passed");


        }

    }
}
