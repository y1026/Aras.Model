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
    [Attributes.ItemType("FileType")]
    public class FileType : Item
    {
        public String Name
        {
            get
            {
                return (String)this.Property("name").Value;
            }
            set
            {
                this.Property("name").Value = value;
            }
        }

        public String Description
        {
            get
            {
                return (String)this.Property("description").Value;
            }
            set
            {
                this.Property("description").Value = value;
            }
        }

        public String Extension
        {
            get
            {
                return (String)this.Property("extension").Value;
            }
            set
            {
                this.Property("extension").Value = value;
            }
        }

        protected override void OnRefresh()
        {
            base.OnRefresh();


        }

        internal FileType(ItemType ItemType, Transaction Transaction)
            : base(ItemType, Transaction)
        {

        }

        internal FileType(ItemType ItemType, IO.Item DBItem)
            : base(ItemType, DBItem)
        {

        }
    }
}
