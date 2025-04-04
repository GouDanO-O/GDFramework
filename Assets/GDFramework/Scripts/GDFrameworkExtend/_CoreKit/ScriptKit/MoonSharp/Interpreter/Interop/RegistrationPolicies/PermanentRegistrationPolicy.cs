﻿using System;

namespace MoonSharp.Interpreter.Interop.RegistrationPolicies
{
    /// <summary>
    /// A registration policy which makes registration permanent and not deletable.
    /// Deregistrations are not allowed and registrations of a new descriptor are not allowed
    /// if a descriptor is already registered for that type.
    /// </summary>
    public class PermanentRegistrationPolicy : IRegistrationPolicy
    {
        /// <summary>
        /// Called to handle the registration or deregistration of a type descriptor. Must return the type descriptor to be registered, or null to remove the registration.
        /// </summary>
        /// <param name="newDescriptor">The new descriptor, or null if this is a deregistration.</param>
        /// <param name="oldDescriptor">The old descriptor, or null if no descriptor was previously registered for this type.</param>
        /// <returns></returns>
        public IUserDataDescriptor HandleRegistration(IUserDataDescriptor newDescriptor,
            IUserDataDescriptor oldDescriptor)
        {
            return oldDescriptor ?? newDescriptor;
        }


        /// <summary>
        /// Allows type automatic registration for the specified type.
        /// NOTE: automatic type registration is NOT recommended.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// True to register the type automatically, false otherwise.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool AllowTypeAutoRegistration(Type type)
        {
            return false;
        }
    }
}