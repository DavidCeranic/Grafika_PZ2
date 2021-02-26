using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PZ2.Model;

public class AddNodes
{
    List<LineEntity> lines = new List<LineEntity>();
    BFS bfs = new BFS();
    List<Putanja> putanje = new List<Putanja>();

    public List<Putanja> NadjiPutanju(Dictionary<long, ElementOfMatrix> allEntity, List<List<ElementOfMatrix>> matrix, List<LineEntity> lines)
    {
        foreach (var item in lines)
        {
            if (allEntity.TryGetValue(item.FirstEnd, out ElementOfMatrix start) && allEntity.TryGetValue(item.SecondEnd, out ElementOfMatrix end))
            {
                for (int i = 0; i < matrix.Count; i++)
                {
                    for (int j = 0; j < matrix[i].Count; j++)
                    {
                        matrix[i][j].IsVisited = false;
                        matrix[i][j].parent = null;
                    }
                }

                var putanja = bfs.NadjiPutanju(allEntity[item.FirstEnd], allEntity[item.SecondEnd], matrix);
                putanja.lineID = item.Id;
                putanje.Add(putanja);

            }
        }

        return putanje;
    }
}

