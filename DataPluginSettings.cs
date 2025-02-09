﻿namespace User.PluginSdk
{
    /// <summary>
    ///     Settings class, make sure it can be correctly serialized using JSON.net
    /// </summary>
    public class DataPluginSettings
    {
        public Theme Theme { get; set; } = Theme.Light;
    }
}