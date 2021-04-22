using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsClient.Models
{
    public class Task
    {
        public long id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime createdAt { get; set; }
        public int duration { get; set; }
        public int state { get; set; }
        public int? timeSpent { get; set; }

        public long? projectId { get; set; }

        public List<User> collaborators { get; set; }


        public string getState()
        {
            switch(state)
            {
                case 0:
                    return "À faire";
                case 1:
                    return "En cours";
                case 2:
                    return "Fait";
                default:
                    return "Rendu";
            }
        }
    }
}
