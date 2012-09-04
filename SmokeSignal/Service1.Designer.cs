namespace SmokeSignal
{
    partial class SmokeSignal
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.theEventLog = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.theEventLog)).BeginInit();
            // 
            // theEventLog
            // 
            this.theEventLog.Log = "Application";
            this.theEventLog.Source = "SmokeSignal";
            // 
            // SmokeSignal
            // 
            this.ServiceName = "SmokeSignalSrvc";
            ((System.ComponentModel.ISupportInitialize)(this.theEventLog)).EndInit();

        }

        #endregion

        public System.Diagnostics.EventLog theEventLog;

    }
}
