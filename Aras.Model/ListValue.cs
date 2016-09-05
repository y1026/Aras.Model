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
    [Attributes.ItemType("Value")]
    public class ListValue : Relationship
    {
        public List List
        {
            get
            {
                return (List)this.Source;
            }
        }

        public String Value
        {
            get
            {
                return (String)this.Property("value").Value;
            }
            set
            {
                this.Property("value").Value = value;
            }
        }

        public String Label
        {
            get
            {
                return (String)this.Property("label").Value;
            }
            set
            {
                this.Property("label").Value = value;
            }
        }

        public override string ToString()
        {
            return Label;
        }

        internal ListValue(RelationshipType RelationshipType, Transaction Transaction, Item Source, Item Related)
            : base(RelationshipType, Transaction, Source, Related)
        {
      
        }

        internal ListValue(RelationshipType RelationshipType, Item Source, IO.Item DBItem)
            : base(RelationshipType, Source, DBItem)
        {

        }
    }
}
