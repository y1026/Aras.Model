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
using Aras.Model;
using System.IO;

namespace Aras.Model.Design.Debug
{
    class Program
    {
        static void OutputOrder(Order Order)
        {
            Console.WriteLine(Order.ItemNumber);

            Part configuredpart = Order.ConfiguredPart;

            foreach (OrderContext ordercontext in Order.Store("v_Order Context"))
            {
                if (ordercontext.Action != Item.Actions.Deleted)
                {
                    Console.WriteLine(ordercontext.Property("value").Value + "\t" + ordercontext.ValueList.Value);
                }
            }

            Console.WriteLine();

            foreach(PartBOM partbom in configuredpart.Store("Part BOM"))
            {
                if (partbom.Action != Item.Actions.Deleted)
                {
                    Console.WriteLine(partbom.Related.Property("name").Value + "\t" + partbom.Quantity.ToString());
                }
            }

            Console.WriteLine();
        }

        static String ItemNumber(int cnt)
        {
            return "DX-" + cnt.ToString().PadLeft(10, '0');
        }

        static int CreateBOM(Part Part, int cnt, int depth, Transaction Transaction)
        {
            int thiscnt = cnt;

            if (depth < 3)
            {
                for(int i=0; i<5; i++)
                {
                    Part childpart = (Part)Part.Session.Store("Part").Create(Transaction);
                    childpart.ItemNumber = ItemNumber(thiscnt);
                    thiscnt++;

                    Part.Store("Part BOM").Create(childpart, Transaction);

                    thiscnt = CreateBOM(childpart, thiscnt, depth + 1, Transaction);
                }
            }

            return thiscnt;
        }

        static void Main(string[] args)
        {
            // Connect to Server
            Server server = new Server("http://localhost/11SP1");
            server.LoadAssembly("Aras.Model.Design");
            Database database = server.Database("VariantsDemo11SP1");
            Session session = database.Login("admin", Server.PasswordHash("innovator"));
            
            // Ensure item_number selected for Parts
            session.ItemType("Part").AddToSelect("item_number");

            String testname = session.ItemType("Part").Icon.Name;
            String testname2 = session.ItemType("Part").OpenIcon.Name;

            using (MemoryStream test = session.ItemType("Part").Icon.Read())
            {
                using (FileStream outst = new FileStream("c:\\temp\\" + session.ItemType("Part").Icon.Name, FileMode.Create))
                {
                    byte[] buffer = new byte[test.Length];
                    int length = test.Read(buffer, 0, (int)test.Length);
                    outst.Write(buffer, 0, length);
                }
            }


            // Query Parts
            Queries.Item partquery = (Queries.Item)session.Store("Part").Query(Aras.Conditions.Eq("item_number", "RJM-999999"));
            partquery.Paging = true;
            partquery.Page = 1;

            // Look at Query Results
            Int32 len = partquery.Count();

            foreach(Part part in partquery)
            {
                Console.WriteLine(part.ItemNumber);
                Console.WriteLine(part.KeyedName);
            }
            
            /*
            // Creating New Parts
            using(Transaction transaction = session.BeginTransaction())
            {
                Design.Part childpart = (Design.Part)session.Store("Part").Create(transaction);
                childpart.ItemNumber = "RJM-999997";

                Design.Part parentpart = (Design.Part)session.Store("Part").Create(transaction);
                parentpart.ItemNumber = "RJM-999996";

                Design.PartBOM partbom = (Design.PartBOM)parentpart.Store("Part BOM").Create(childpart, transaction);
                partbom.Quantity = 3.0;

                transaction.Commit();
            } */
        }
    }
}
