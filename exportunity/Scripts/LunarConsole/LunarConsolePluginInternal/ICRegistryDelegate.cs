using LunarConsolePlugin;

namespace LunarConsolePluginInternal;

public interface ICRegistryDelegate
{
	void OnActionRegistered(CRegistry registry, CAction action);

	void OnActionUnregistered(CRegistry registry, CAction action);

	void OnVariableRegistered(CRegistry registry, CVar cvar);

	void OnVariableUpdated(CRegistry registry, CVar cvar);
}
