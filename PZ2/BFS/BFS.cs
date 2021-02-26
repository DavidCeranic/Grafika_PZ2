using PZ2;
using PZ2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Putanja
{
    public List<ElementOfMatrix> points = new List<ElementOfMatrix>();
    public long lineID;
}



public class BFS
{
    Putanja GetPathFromNodeParents(ElementOfMatrix element)
    {
        Putanja putanja = new Putanja();
        putanja.points.Add(element);

        while (element.parent != null)
        {
            putanja.points.Add(element.parent);
            element = element.parent;
        }

        return putanja;
    }


    public Putanja NadjiPutanju(ElementOfMatrix firstEnd, ElementOfMatrix sechondEnd, List<List<ElementOfMatrix>> matrix)
    {
        Putanja path = new Putanja();


        Queue<ElementOfMatrix> queue = new Queue<ElementOfMatrix>();

        queue.Enqueue(firstEnd);

        while (queue.Count > 0)
        {
            var temp = queue.Dequeue();
            temp.IsVisited = true;
            int startX = temp.X / MainWindow.VelicinaPolja;
            int startY = temp.Y / MainWindow.VelicinaPolja;

            if (temp == sechondEnd)
            {
                return GetPathFromNodeParents(temp);
            }


            if (startX + 1 < 400)
            {
                var n = matrix[startX + 1][startY];
                if (n.IsLineOnElement == false && n.IsVisited == false && n.IsExamined == false)
                {
                    n.IsVisited = true;
                    n.parent = temp;

                    queue.Enqueue(n);
                }
            }

            if (startX - 1 >= 0 && startX - 1 < 400)
            {
                var n = matrix[startX - 1][startY];
                if (n.IsLineOnElement == false && n.IsVisited == false && n.IsExamined == false)
                {
                    n.IsVisited = true;
                    n.parent = temp;

                    queue.Enqueue(n);
                }
            }

            if (startY + 1 < 400)
            {
                var n = matrix[startX][startY + 1];
                if (n.IsLineOnElement == false && n.IsVisited == false && n.IsExamined == false)
                {
                    n.IsVisited = true;
                    n.parent = temp;

                    queue.Enqueue(n);
                }
            }

            if (startY - 1 >= 0 && startY - 1 < 400)
            {
                var n = matrix[startX][startY - 1];
                if (n.IsLineOnElement == false && n.IsVisited == false && n.IsExamined == false)
                {
                    n.IsVisited = true;
                    n.parent = temp;

                    queue.Enqueue(n);
                }
            }
        }

        return new Putanja();
    }




    public static ElementOfMatrix FreeEmptyPosition(ElementOfMatrix firstEnd, List<List<ElementOfMatrix>> matrix)
    {
        for (int i = 0; i < matrix.Count; i++)
        {
            for (int j = 0; j < matrix[i].Count; j++)
            {
                matrix[i][j].IsVisited = false;
                matrix[i][j].parent = null;
            }
        }

        Queue<ElementOfMatrix> queue = new Queue<ElementOfMatrix>();

        queue.Enqueue(firstEnd);

        while (queue.Count > 0)
        {
            var temp = queue.Dequeue();
            temp.IsVisited = true;
            int startX = temp.X / MainWindow.VelicinaPolja;
            int startY = temp.Y / MainWindow.VelicinaPolja;

            if (temp.powerEntities.Count == 0)
            {
                return temp;
            }


            if (startX + 1 < 400)
            {
                var n = matrix[startX + 1][startY];
                if (n.IsLineOnElement == false && n.IsVisited == false && n.IsExamined == false)
                {
                    n.IsVisited = true;
                    n.parent = temp;

                    queue.Enqueue(n);
                }
            }

            if (startX - 1 >= 0 && startX - 1 < 400)
            {
                var n = matrix[startX - 1][startY];
                if (n.IsLineOnElement == false && n.IsVisited == false && n.IsExamined == false)
                {
                    n.IsVisited = true;
                    n.parent = temp;

                    queue.Enqueue(n);
                }
            }

            if (startY + 1 < 400)
            {
                var n = matrix[startX][startY + 1];
                if (n.IsLineOnElement == false && n.IsVisited == false && n.IsExamined == false)
                {
                    n.IsVisited = true;
                    n.parent = temp;

                    queue.Enqueue(n);
                }
            }

            if (startY - 1 >= 0 && startY - 1 < 400)
            {
                var n = matrix[startX][startY - 1];
                if (n.IsLineOnElement == false && n.IsVisited == false && n.IsExamined == false)
                {
                    n.IsVisited = true;
                    n.parent = temp;

                    queue.Enqueue(n);
                }
            }
        }

        return null;
    }

}

