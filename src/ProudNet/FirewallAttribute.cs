using System;

namespace ProudNet
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class FirewallAttribute : Attribute
    {
        public Type FirewallRuleType { get; set; }
        public object[] Parameters { get; set; }
        public bool Invert { get; set; }

        /// <summary>
        /// Adds a firewall rule to the message handler
        /// </summary>
        /// <param name="firewallRuleType">The type that implements the rule</param>
        /// <param name="parameters">Constants that get passed to the constructor</param>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Make sure to only define ONE constructor.
        ///
        /// Dependency Injection is used for any left over parameters.
        /// e.g. if you pass 1 paramter to a constructor with 2 parameters
        /// the second parameter will be resolved using Dependency Injection.
        /// </remarks>
        public FirewallAttribute(Type firewallRuleType, params object[] parameters)
        {
            if (!typeof(IFirewallRule).IsAssignableFrom(firewallRuleType))
                throw new ArgumentException($"Type must implement {nameof(IFirewallRule)}", nameof(firewallRuleType));

            FirewallRuleType = firewallRuleType;
            Parameters = parameters;
        }
    }
}
