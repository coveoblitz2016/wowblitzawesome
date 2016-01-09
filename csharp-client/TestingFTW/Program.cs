using System;
using Coveo;
using Coveo.Bot;
using CoveoBlitz;

namespace TestingFTW
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Tile[][] map = {
                new[] { Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE },
                new[] { Tile.FREE, Tile.HERO_1, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE },
                new[] { Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE },
                new[] { Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE },
                new[] { Tile.TAVERN, Tile.FREE, Tile.FREE, Tile.FREE, Tile.GOLD_MINE_NEUTRAL, Tile.FREE },
                new[] { Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE }
            };

            EvenBestChoice moveGenerator = new EvenBestChoice();
            AStar aStar = new AStar();

            Hero hero = new Hero {
                life = 100,
                pos = new Pos(1, 1),
                id = 1
            };

            while (hero.pos.x != 4 || hero.pos.y != 4) {
                GameState state = new GameState();
                state.myHero = hero;
                state.board = map;

                string nextmove = moveGenerator.BestMove(state, aStar);
                Console.WriteLine(nextmove);

                map[hero.pos.x][hero.pos.y] = Tile.FREE;

                switch (nextmove) {
                    case Direction.East:
                        hero.pos.y += 1;
                        break;
                    case Direction.West:
                        hero.pos.y -= 1;
                        break;
                    case Direction.North:
                        hero.pos.x -= 1;
                        break;
                    case Direction.South:
                        hero.pos.x += 1;
                        break;
                }

                map[hero.pos.x][hero.pos.y] = Tile.HERO_1;

            }

            


            Console.ReadKey();
        }

        private static void AstarTest()
        {
            AStar lol = new AStar();

            float dist = lol.GetDistance(new Pos { x=4, y=4 }, new Pos { x = 0, y = 0 });
            Console.WriteLine(dist);


            Tile[][] tiles = {
                new [] { Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE },
                new [] { Tile.FREE, Tile.FREE, Tile.IMPASSABLE_WOOD, Tile.FREE, Tile.FREE },
                new [] { Tile.FREE, Tile.IMPASSABLE_WOOD, Tile.FREE, Tile.FREE, Tile.FREE },
                new [] { Tile.FREE, Tile.FREE, Tile.IMPASSABLE_WOOD, Tile.FREE, Tile.FREE },
                new [] { Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE, Tile.FREE }
            };

            foreach (Tile[] row in tiles) {
                foreach (Tile tile in row) {
                    switch (tile) {
                        case Tile.FREE:
                            Console.Write("_ ");
                            break;
                        default:
                            Console.Write("# ");
                            break;
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine("=====================");
            Pos cur = new Pos() {x = 0, y=0};
            Pos target = new Pos() {x = 4, y=4};

            int nbTours = 0;

            while (cur.x != target.x || cur.y != target.y) {
                PathData result = lol.pathTo(target, cur, tiles);

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
