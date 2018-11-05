namespace Netsphere.Database.Helpers
{
    public abstract class DatabaseObject
    {
        public bool IsDirty { get; private set; }
        public bool Exists { get; private set; }

        public void SetDirtyState(bool state)
        {
            IsDirty = state;
        }

        public void SetExistsState(bool state)
        {
            Exists = state;
        }

        protected void SetIfChanged<T>(ref T field, T value)
        {
            if (field.Equals(value))
                return;

            SetDirtyState(true);
            field = value;
        }
    }
}
