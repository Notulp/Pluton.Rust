﻿namespace Pluton.Rust.Objects.PlutonUI
{
    public interface IComponent
    {
        float fadeIn { get; }

        JSON.Object obj { get; }

        string type { get; }
    }
}
