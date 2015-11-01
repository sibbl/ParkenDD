namespace ParkenDD.Messages
{
    public class SettingChangedMessage
    {
        public string SettingPropertyName { get; private set; }

        public SettingChangedMessage(string name)
        {
            SettingPropertyName = name;
        }

        public bool IsSetting(string name)
        {
            return SettingPropertyName.Equals(name);
        }
    }
}
