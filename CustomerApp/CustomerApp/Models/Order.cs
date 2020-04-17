using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using Realms;

namespace CustomerApp.Models
{
    public class Order : RealmObject
    {
        [PrimaryKey]
        public string _id { get; set; }

        public bool send_to_kitchen { get; set; }

        public string employee_id { get; set; }

        public int table_number { get; set; }

        public IList<OrderItem> menuItems { get; }

        public IList<IngredientCount> IngredientTotals { get; }

        public System.Threading.Tasks.Task UpdateIngredientTotal()
        {
            // Clear out existing list
            RealmManager.RemoveAll<IngredientCount>();

            // Sum all ingredients for all menu items
            foreach(OrderItem o in menuItems)
            {
                foreach(string ID in o.ingredients)
                {
                    List<IngredientCount> container = (IngredientTotals.Where((IngredientCount i) => i._id == ID).ToList()); // Returns an empty list if no matching ingredientCount object is found

                    int index = -1;
                    if(!container.Count.Equals(0))
                        index = IngredientTotals.IndexOf(container[0]);

                    if (index == -1) // New ingredient
                    {
                        IngredientCount temp = new IngredientCount();
                        temp._id = ID;
                        temp.quantity = 0;
                        RealmManager.Write(() => IngredientTotals.Add(temp));
                        index = IngredientTotals.Count - 1;
                    }

                    RealmManager.Write(() => IngredientTotals[index].quantity++);
                }
                
            }

            return System.Threading.Tasks.Task.CompletedTask;
        }


        public Order() 
        {
            //IngredientTotals = new List<Ingredient>();
        }

        // Copy constructor
        public Order(Order o)
        {
            _id = o._id;
            // Deep copy of each menu item
            foreach (OrderItem m in o.menuItems)
                menuItems.Add(new OrderItem(m));

            //waitstaff_id = o.waitstaff_id;
            send_to_kitchen = o.send_to_kitchen;

            IngredientTotals = new List<IngredientCount>();

            UpdateIngredientTotal();
        }

    }
}
