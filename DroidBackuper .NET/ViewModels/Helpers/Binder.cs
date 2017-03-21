using System;


namespace DroidBackuper.NET.ViewModels.Helpers
{
    public class Binder
    {
        private readonly Func<bool> ruleDelegate;
        private readonly string message;

        internal Binder(Func<bool> ruleDelegate, string message)
        {
            this.ruleDelegate = ruleDelegate;
            this.message = message;

            IsDirty = true;
        }

        internal string Error { get; set; }
        internal bool HasError { get; set; }

        internal bool IsDirty { get; set; }

        internal void Update()
        {
            if (!IsDirty)
                return;

            Error = null;
            HasError = false;
            try
            {
                if (!ruleDelegate())
                {
                    Error = message;
                    HasError = true;
                }
            }
            catch (Exception e)
            {
                Error = e.Message;
                HasError = true;
            }
        }
    }
}
