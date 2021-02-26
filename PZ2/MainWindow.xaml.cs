using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using PZ2.Model;
using Brushes = System.Drawing.Brushes;
using Pen = System.Drawing.Pen;
using Size = System.Drawing.Size;

namespace PZ2
{
    public partial class MainWindow : Window
    {
        private List<SubstationEntity> substationList = new List<SubstationEntity>();
        private List<NodeEntity> nodeList = new List<NodeEntity>();
        private List<SwitchEntity> switchList = new List<SwitchEntity>();
        public Dictionary<long, LineEntity> lineList = new Dictionary<long, LineEntity>();
        public Dictionary<long, ElementOfMatrix> allEntity = new Dictionary<long, ElementOfMatrix>();
        Dictionary<long, List<Model.LineSegment>> Lines = new Dictionary<long, List<Model.LineSegment>>();

        private List<List<ElementOfMatrix>> Matrix = new List<List<ElementOfMatrix>>(400);
        double minX = Int32.MaxValue;
        double minY = Int32.MaxValue;
        double maxX = Int32.MinValue;
        double maxY = Int32.MinValue;

        public static int VelicinaPolja;
        public static int Pomeraj = 120;
        public int velicinaMatrice = 400;

        public MainWindow()
        {
            InitializeComponent();

            VelicinaPolja = (int)canvas.Width / velicinaMatrice;
            CreateMatrix();
            Parser();
        }

        private void Parser()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");

            XmlNodeList nodeListSubstation = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            XmlNodeList nodeListSwitch = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            XmlNodeList nodeListNode = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");

            XmlNodeList tempList;
            double noviX, noviY;

            var minMax = FindMinMax(new List<XmlNodeList>() { nodeListSubstation, nodeListSwitch, nodeListNode });
            minX = minMax.min.x;
            minY = minMax.min.y;
            maxX = minMax.max.x;
            maxY = minMax.max.y;
            var distance = Math.Sqrt(Math.Pow(minMax.max.x - minMax.min.x, 2) + Math.Pow(minMax.max.y - minMax.min.y, 2));
            var scale = canvas.Width / distance;

            tempList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");

            foreach (XmlNode node in tempList)
            {
                SubstationEntity substationEntity = new SubstationEntity();
                substationEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                substationEntity.Name = node.SelectSingleNode("Name").InnerText;

                ToLatLon(double.Parse(node.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture), double.Parse(node.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture), 34, out noviX, out noviY);
                substationEntity.X = noviX;
                substationEntity.Y = noviY;

                substationList.Add(substationEntity);
                var dot = CreateDot(substationEntity, scale, System.Windows.Media.Color.FromRgb(255, 0, 0));
                allEntity.Add(substationEntity.Id, dot);
            }


            tempList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");

            foreach (XmlNode node in tempList)
            {
                NodeEntity nodeEntity = new NodeEntity();
                nodeEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                nodeEntity.Name = node.SelectSingleNode("Name").InnerText;

                ToLatLon(double.Parse(node.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture), double.Parse(node.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture), 34, out noviX, out noviY);
                nodeEntity.X = noviX;
                nodeEntity.Y = noviY;

                nodeList.Add(nodeEntity);
                var dot = CreateDot(nodeEntity, scale, System.Windows.Media.Color.FromRgb(0, 255, 0));
                allEntity.Add(nodeEntity.Id, dot);
            }


            tempList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            foreach (XmlNode node in tempList)
            {
                SwitchEntity switchEntity = new SwitchEntity();

                switchEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                switchEntity.Name = node.SelectSingleNode("Name").InnerText;
                switchEntity.Status = node.SelectSingleNode("Status").InnerText;

                ToLatLon(double.Parse(node.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture), double.Parse(node.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture), 34, out noviX, out noviY);
                switchEntity.X = noviX;
                switchEntity.Y = noviY;

                switchList.Add(switchEntity);
                var dot = CreateDot(switchEntity, scale, System.Windows.Media.Color.FromRgb(0, 0, 255));
                allEntity.Add(switchEntity.Id, dot);
            }

            AddNodes add = new AddNodes();

            tempList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
            foreach (XmlNode node in tempList)
            {
                LineEntity lineEntity = new LineEntity();

                lineEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                lineEntity.Name = node.SelectSingleNode("Name").InnerText;
                if (node.SelectSingleNode("IsUnderground").InnerText.Equals("true"))
                    lineEntity.IsUnderground = true;
                else
                    lineEntity.IsUnderground = false;
                lineEntity.R = float.Parse(node.SelectSingleNode("R").InnerText, CultureInfo.InvariantCulture);
                lineEntity.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
                lineEntity.LineType = node.SelectSingleNode("LineType").InnerText;
                lineEntity.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText, CultureInfo.InvariantCulture);
                lineEntity.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText, CultureInfo.InvariantCulture);
                lineEntity.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText, CultureInfo.InvariantCulture);

                lineList.Add(lineEntity.Id, lineEntity);
            }

            foreach (var putanje in add.NadjiPutanju(allEntity, Matrix, lineList.Values.ToList()))
            {
                Lines[putanje.lineID] = new List<Model.LineSegment>();
                for (int i = 0; i < putanje.points.Count - 1; i++)
                {
                    if (putanje.points[i].IsLineOnElement == false || putanje.points[i + 1].IsLineOnElement == false)
                    {
                        Polyline p = new Polyline();
                        p.Points.Add(new System.Windows.Point(putanje.points[i].X, putanje.points[i].Y));
                        p.Points.Add(new System.Windows.Point(putanje.points[i + 1].X, putanje.points[i + 1].Y));
                        putanje.points[i].LinePart(putanje.lineID);

                        Lines[putanje.lineID].Add(new Model.LineSegment() { line = p, p1 = putanje.points[i], p2 = putanje.points[i + 1] });


                        if (putanje.points[i].IsLineOnElement && putanje.points[i].powerEntities.Count == 0)
                        {
                            putanje.points[i].AssignCross();
                        }
                        putanje.points[i].IsLineOnElement = true;
                        p.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
                        p.StrokeThickness = 0.5;
                        canvas.Children.Add(p);
                        p.MouseRightButtonDown += putanje.points[i].RightClick;
                        p.MouseEnter += putanje.points[i].ToolTipLine;
                    }
                    else
                    {
                        bool found = false;
                        foreach (var item in Lines.Values)
                        {
                            if (found)
                                break;
                            foreach (var l in item)
                            {
                                if (putanje.points[i] == l.p1 && putanje.points[i + 1] == l.p2)
                                {
                                    found = true;
                                    Lines[putanje.lineID].Add(l);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CreateMatrix()
        {
            int razmak = (int)canvas.Width / 400;
            for (int i = 0; i < 400; i++)
            {
                Matrix.Add(new List<ElementOfMatrix>(400));
                for (int j = 0; j < 400; j++)
                {
                    Matrix[i].Add(new ElementOfMatrix(1f, canvas, razmak * i, razmak * j, clickLine));
                }
            }
        }


        (MyPoint min, MyPoint max, List<MyPoint>) FindMinMax(List<XmlNodeList> xmlPointDataHolders)
        {
            List<MyPoint> sortPoints = new List<MyPoint>();
            foreach (var pointDataColliection in xmlPointDataHolders)
            {
                foreach (XmlNode node in pointDataColliection)
                {
                    sortPoints.Add(new MyPoint()
                    {
                        x = double.Parse(node.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture),
                        y = double.Parse(node.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture),
                        id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture)
                    });
                }
            }

            sortPoints.ForEach(s => ToLatLon(s.x, s.y, 34, out s.x, out s.y));
            var ordered = sortPoints.OrderBy(s => s.x).ThenBy(s => s.y).ToList();
            return (ordered.First(), ordered.Last(), sortPoints);
        }


        public static int Convert(double point, double scale, double min, double size, double width)
        {
            int a = (int)(((point - min) * scale / width) * size % width);
            return a;
        }

        ElementOfMatrix CreateDot(PowerEntity entity, double scale, System.Windows.Media.Color color)
        {

            int i = Convert(entity.X, scale * 50, minX, canvas.Width / 400, canvas.Width);
            int j = Convert(entity.Y, scale * 50, minY, canvas.Width / 400, canvas.Height);

            var examinedSpot = Matrix[i + 170][j + 170];
            if (examinedSpot.powerEntities.Count == 0)
            {
                examinedSpot.Create(entity, color);
                return examinedSpot;
            }
            else
            {
                var next = BFS.FreeEmptyPosition(examinedSpot, Matrix);
                if (next == null)
                {
                    examinedSpot.Create(entity, color);
                    return examinedSpot;
                }
                else
                {
                    next.Create(entity, color);
                    Matrix[i + 170][j + 170].Create(entity, color);
                    return next;
                }

            }
        }


        public static string ToopTip(List<PowerEntity> entity)
        {
            string type;
            string tooltip = "";

            foreach (var item in entity)
            {
                if (item.GetType().ToString() != "PZ2.Model.SubstationEntity")
                    type = "Substation";
                else if (item.GetType().ToString() != "PZ2.Model.NodeEntity")
                    type = "Node";
                else
                    type = "Switch";



                if (item.GetType().ToString() != "PZ2.Model.SwitchEntity")
                {
                    tooltip += "Type: " + type + "\n";
                    tooltip += "ID: " + item.Id + "\n";
                    tooltip += "Name: " + item.Name + "\n";
                    tooltip += "\n";
                }
                else
                {
                    tooltip += "Type: " + type + "\n";
                    tooltip += "ID: " + item.Id + "\n";
                    tooltip += "Name: " + item.Name + "\n";
                    tooltip += "Status: " + (item as SwitchEntity).Status;
                    tooltip += "\n";
                }
            }

            return tooltip;
        }

        public void clickLine(long lineId, System.Windows.Media.Color color, bool draw)
        {
            if (Lines.TryGetValue(lineId, out List<Model.LineSegment> lineSegments))
            {
                foreach (var item in lineSegments)
                {
                    item.line.Stroke = new SolidColorBrush(color);
                }

                var line = lineList[lineId];
                var firstEnd = allEntity[line.FirstEnd];
                var sechondEnd = allEntity[line.SecondEnd];
                firstEnd.Draw(draw);
                sechondEnd.Draw(draw);
            }
        }

        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }
    }
}
