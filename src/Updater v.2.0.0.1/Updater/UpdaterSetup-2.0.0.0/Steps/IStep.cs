namespace UpdaterSetup_2._0._0._0 {
    internal interface IStep {
        string Name { get; }
        string Instruction { get; }
        void OnActivate();
        void OnDeactivate();
        void Cancel();
    }
}