using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmpolyeeManagment.Models.Interfaces
{
   public interface IEmployeeCollection<Entity>
    {
        Entity Find(int id);
        IEnumerable<Entity> GetAll();
        Entity Add(Entity entity);
        void Delete(int id);
        Entity Update(Entity entity); 

    } 
}
