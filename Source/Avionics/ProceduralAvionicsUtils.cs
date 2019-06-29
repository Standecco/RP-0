namespace RP0.ProceduralAvionics
{
<<<<<<< master
    static class ProceduralAvionicsUtils
    {
        private static bool _enableLogging = true;
=======
	static class ProceduralAvionicsUtils
	{
		private static bool _enableLogging = true;
>>>>>>> ProcAvionicsTooling

        private const string logPreix = "[ProcAvi] - ";
        public static void Log(params string[] message)
        {
            if (_enableLogging) {
                var builder = StringBuilderCache.Acquire();
                builder.Append(logPreix);
                foreach (string part in message) {
                    builder.Append(part);
                }
                UnityEngine.Debug.Log(builder.ToStringAndRelease());
            }
        }

        public static void Log(params object[] parts)
        {
            if (_enableLogging) {
                var builder = StringBuilderCache.Acquire();
                builder.Append(logPreix);
                foreach (object part in parts) {
                    builder.Append(part.ToString());
                }
                UnityEngine.Debug.Log(builder.ToStringAndRelease());
            }
        }

    }
}
