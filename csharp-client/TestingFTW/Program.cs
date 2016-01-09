using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coveo;
using Coveo.Bot;
using CoveoBlitz;

namespace TestingFTW
{
    class Program
    {
        static void Main(string[] args)
        {
            AStar lol = new AStar();

            float dist = lol.GetDistance(new Pos { x=4, y=4 }, new Pos { x = 0, y = 0 });
            Console.WriteLine(dist);


            Tile[][] tiles = {
                new [] { Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE },
                new [] { Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE },
                new [] { Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE },
                new [] { Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE },
                new [] { Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE }
            };

            foreach (Tile[] row in tiles) {
                foreach (Tile tile in row) {
                    Console.Write(tile);
                }
                Console.WriteLine();
            }

            

            PathData result = lol.pathTo(new Pos { x = 0, y = 0 }, new Pos { x = 4, y = 4 }, tiles);
            Console.WriteLine("Dist:{0} - Direction: {1}", result.distance, result.nextDirection);

            result = lol.pathTo(new Pos { x = 4, y = 0 }, new Pos { x = 4, y = 4 }, tiles);
            Console.WriteLine("Dist:{0} - Direction: {1}", result.distance, result.nextDirection);

            result = lol.pathTo(new Pos { x = 0, y = 4 }, new Pos { x = 4, y = 4 }, tiles);
            Console.WriteLine("Dist:{0} - Direction: {1}", result.distance, result.nextDirection);

            result = lol.pathTo(new Pos { x = 4, y = 4 }, new Pos { x = 4, y = 0 }, tiles);
            Console.WriteLine("Dist:{0} - Direction: {1}", result.distance, result.nextDirection);

            result = lol.pathTo(new Pos { x = 4, y = 4 }, new Pos { x = 0, y = 4 }, tiles);
            Console.WriteLine("Dist:{0} - Direction: {1}", result.distance, result.nextDirection);

            result = lol.pathTo(new Pos { x = 4, y = 4 }, new Pos { x = 0, y = 0 }, tiles);
            Console.WriteLine("Dist:{0} - Direction: {1}", result.distance, result.nextDirection);

            Console.WriteLine("=====================");
            Pos cur = new Pos() {x = 0, y=0};
            Pos target = new Pos() {x = 4, y=4};

            int nbTours = 0;

            while (cur.x != target.x || cur.y != target.y) {
                result = lol.pathTo(target, cur, tiles);

                switch(result.nextDirection) {
                    case Direction.East:
                        cur.y += 1;
                        break;
                    case Direction.West:
                        cur.y -= 1;
                        break;
                    case Direction.North:
                        cur.x -= 1;
                        break;
                    case Direction.South:
                        cur.x += 1;
                        break;
                }

                Console.WriteLine("Dist:{0} - Direction: {1}", result.distance, result.nextDirection);
                Console.WriteLine("CurPos = {0}-{1}", cur.x, cur.y);

                if (nbTours++ > 10) {
                    break;
                }
            }

            Console.ReadKey();
        }
    }
}
