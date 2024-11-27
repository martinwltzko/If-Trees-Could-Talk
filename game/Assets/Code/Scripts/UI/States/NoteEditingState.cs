namespace Code.Scripts.UI
{
    public sealed class NoteEditingState : BehaviourState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Controller.OnNoteEditing(true);
        }

        public override void OnExit()
        {
            base.OnExit();
            Controller.OnNoteEditing(false);
        }
    }
}