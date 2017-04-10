﻿/*  
  Aras.Model provides a .NET cient library for Aras Innovator

  Copyright (C) 2015 Processwall Limited.

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU Affero General Public License as published
  by the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Affero General Public License for more details.

  You should have received a copy of the GNU Affero General Public License
  along with this program.  If not, see http://opensource.org/licenses/AGPL-3.0.
 
  Company: Processwall Limited
  Address: The Winnowing House, Mill Lane, Askham Richard, York, YO23 3NW, United Kingdom
  Tel:     +44 113 815 3440
  Email:   support@processwall.com
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.Model
{
    internal abstract class Action
    {
        internal Transaction Transaction { get; private set; }

        private String _name;
        internal String Name 
        { 
            get
            {
                return this._name;
            }
            set
            {
                if (value != null)
                {
                    if ((this._name != null) && (this._name.Equals("add")) && value.Equals("delete"))
                    {
                        // No Action needed on database
                        this._name = null;
                    }
                    else
                    {
                        this._name = value;
                    }
                }
                else
                {
                    throw new Exceptions.ArgumentException("Must specify Name of Action");
                }
            }
        }

        protected Item Item { get; set; }

        private Dictionary<String, Actions.Relationship> RelationshipsCache;

        internal void AddRelationship(Actions.Relationship Relationship)
        {
            this.RelationshipsCache[Relationship.Item.ID] = Relationship;
        }

        internal void RemoveRelationship(String ID)
        {
            this.RelationshipsCache.Remove(ID);
        }

        protected IEnumerable<Actions.Relationship> Relationships
        {
            get
            {
                return this.RelationshipsCache.Values;
            }
        }

        protected Boolean Completed;

        internal abstract void Rollback();

        internal abstract IO.Item Commit();

        internal abstract void UpdateStore();

        protected IO.Item BuildItem()
        {
            // Create IO Item
            IO.Item dbitem = new IO.Item(this.Item.ItemType.Name, this.Name);
            dbitem.ID = this.Item.ID;

            // Add Properties
            foreach (Property prop in this.Item.Properties)
            {
                if (!prop.Type.ReadOnly && (prop.Modified))
                {
                    if (prop is Properties.Item)
                    {
                        // Check that related Item is processed
                        Properties.Item itemprop = (Properties.Item)prop;

                        if (itemprop.Value != null)
                        {
                            Action itempropaction = this.Transaction.Get(((Model.Item)itemprop.Value).ID);

                            if (itempropaction != null)
                            {
                                itempropaction.Commit();
                            }
                        }
                    }

                    dbitem.SetProperty(prop.Type.Name, prop.DBValue);
                }
            }

            // Add Relations
            foreach (Actions.Relationship relationshipaction in this.RelationshipsCache.Values)
            {
                IO.Item dbrelationship = relationshipaction.Commit();
                dbitem.AddRelationship(dbrelationship);
            }

            return dbitem;
        }

        protected IO.Response SendItem(IO.Item DBItem)
        {
            // Send to Server
            IO.Request request = this.Transaction.Session.IO.Request(IO.Request.Operations.ApplyItem, DBItem);
            return request.Execute();
        }

        internal void CheckLock(Boolean UnLock)
        {
            if (this.Item.Action != Model.Item.Actions.Delete)
            {
                if (UnLock)
                {
                    // Unlock
                    this.Item.UnLock();
                }
                else
                {
                    // Ensure Locked
                    this.Item.Update(this.Transaction);
                }
            }
        }

        protected void UpdateItem(IO.Item DBItem)
        {
            // Update Properties
            this.Item.UpdateProperties(DBItem);

            foreach (Actions.Relationship relation in this.RelationshipsCache.Values)
            {
                Boolean found = false;

                foreach (IO.Item dbrelationship in DBItem.Relationships)
                {
                    if (dbrelationship.ID.Equals(relation.Item.ID))
                    {
                        relation.Item.UpdateProperties(dbrelationship);
                        found = true;
                    }
                }

                if (!found && !relation.Name.Equals("delete"))
                {
                    relation.Item.UpdateProperties(null);
                }
            }
        }

        public override string ToString()
        {
            return this.Name + " " + this.Item.ID;
        }

        internal Action(Transaction Transaction, String Name, Item Item)
        {
            this.RelationshipsCache = new Dictionary<string, Actions.Relationship>();
            this.Transaction = Transaction;
            this.Name = Name;
            this.Item = Item;
            this.Completed = false;
        }
    }
}
