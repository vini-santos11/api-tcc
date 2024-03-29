﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enumerables.Base
{
    public abstract class BaseEnumeration
    {
        public string Name { get; private set; }
        public int Id { get; private set; }
        protected BaseEnumeration(int id, string name) => (Id, Name) = (id, name);
        public override string ToString() => Name;
        public static IEnumerable<T> GetAll<T>() where T : BaseEnumeration =>
            typeof(T).GetFields(BindingFlags.Public |
                                BindingFlags.Static |
                                BindingFlags.DeclaredOnly)
                     .Select(f => f.GetValue(null))
                     .Cast<T>();

        public override bool Equals(object obj)
        {
            if (obj is not BaseEnumeration otherValue)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object other) => Id.CompareTo(((BaseEnumeration)other).Id);

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static T FindById<T>(int id) where T : BaseEnumeration
        {
            return GetAll<T>().Where(i => i.Id == id).FirstOrDefault();
        }
        public static T FindByName<T>(string name) where T : BaseEnumeration
        {
            return GetAll<T>().Where(i => i.Name.ToUpper() == name.ToUpper()).FirstOrDefault();
        }
    }
}
