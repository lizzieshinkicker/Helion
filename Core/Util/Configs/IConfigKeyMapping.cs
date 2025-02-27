using Helion.Util.Configs.Impl;
using Helion.Window;
using Helion.Window.Input;
using System.Collections.Generic;

namespace Helion.Util.Configs;

/// <summary>
/// A bi-directional mapping of keys to commands.
/// </summary>
public interface IConfigKeyMapping
{
    /// <summary>
    /// Whether any keys have been set. This is intended to be a marker for
    /// the choice to save or not when something has changed.
    /// </summary>
    public bool Changed { get; }

    /// <summary>
    /// Adds a key to be mapped. This is a bi-directional mapping.
    /// </summary>
    /// <param name="key">The key to add.</param>
    /// <param name="command">The command for the key.</param>
    bool Add(Key key, string command);
    bool Remove(Key key);
    bool Remove(Key key, string command);

    /// <summary>
    /// Consumes the key for the mapped command if it is currently pressed.
    /// </summary>
    /// <param name="command">The command, which is not case sensitive.</param>
    /// <param name="input">The consumable input.</param>
    /// <returns>True if it was found to be pressed and was consumed, false if
    /// not or if something else consumed it.</returns>
    bool ConsumeCommandKeyPress(string command, IConsumableInput input, out int scrollAmount);

    /// <summary>
    /// Consumes any of the specified commands if currently pressed
    /// This will only consume the _first_ matching command key.
    /// </summary>
    /// <param name="input">The consumable input</param>
    /// <param name="commands">The commands to consume</param>
    /// <returns>True if any of the specified commands were found and consumed</returns>
    bool ConsumeCommandKeyPress(IConsumableInput input, params string[] commands);

    /// <summary>
    /// Consumes the key for the mapped command if it is currently down.
    /// </summary>
    /// <param name="command">The command, which is not case sensitive.</param>
    /// <param name="input">The consumable input.</param>
    /// <returns>True if it was found to be down and was consumed, false if
    /// not or if something else consumed it.</returns>
    bool ConsumeCommandKeyDown(string command, IConsumableInput input, out int scrollAmount, out Key key);

    /// <summary>
    /// Consumes the key for the mapped command if it is currently down.
    /// </summary>
    /// <param name="command">The command, which is not case sensitive.</param>
    /// <param name="input">The consumable input.</param>
    /// <returns>True if it was found to be down.</returns>
    bool IsCommandKeyDown(string command, IConsumableInput input);

    /// <summary>
    /// Consumes the key for the mapped command if it is currently down.
    /// </summary>
    /// <param name="command">The command, which is not case sensitive.</param>
    /// <param name="input">The consumable input.</param>
    /// <returns>True if it was pressed and consumed or continously held down.</returns>
    bool ConsumeCommandKeyPressOrContinuousHold(string command, IConsumableInput input, out int scrollAmount);

    /// <summary>
    /// Unbinds all commands and keys that match this key. This means the
    /// key will be removed, and any alias that would point onto that key
    /// would be removed. This does not remove bindings to other keys.
    /// </summary>
    /// <remarks>
    /// For example, suppose we had { A -> ["cmdA", "cmdB"], B -> ["cmdA", "cmdB"] }.
    /// If we UnbindAll(A), then the map would look like { B -> ["cmdA", "cmdB"] }.
    /// Likewise, any lookups for "cmdA" would return B only after this is invoked.
    /// </remarks>
    /// <param name="key">The key to unbind.</param>
    void UnbindAll(Key key);

    IList<KeyCommandItem> GetKeyMapping();

    /// <summary>
    /// Restore default bindings for specified command
    /// </summary>
    void ReloadDefaults(string command);

    /// <summary>
    /// Reloads all default key bindings
    /// </summary>
    void SetInitialDefaultKeyBindings();

    /// <summary>
    /// Ensures that the user has some way to get back to the menus even if they have unbound all keys.
    /// </summary>
    void EnsureMenuKey();
}
