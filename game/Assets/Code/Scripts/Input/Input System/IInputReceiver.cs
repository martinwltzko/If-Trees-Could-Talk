namespace GameSystems.InputSystem
{
    public interface IInputReceiver
    {
        public string InputIdentifier { get; }
        public void UpdateInput(FrameInput frameInput);
    }
}
