using CoveoBlitz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coveo
{
    class Disjkstra : IPathfinder
    {
        public PathData pathTo(Pos target, Pos currentLocation, Tile[][] board, int spikeCost = 5)
        {
            int resultCost = 0;
            PathData pathData = new PathData();
            int size = board.GetLength(0);
            int[][] pointValues = new int[size][];
            for (int x = 0; x < size; x++) 
            {
               pointValues[x] = new int[size];
            }
            pointValues[currentLocation.x][currentLocation.y] = -1;
            Queue<Pos> newMarkedPoints = new Queue<Pos>();
            newMarkedPoints.Enqueue(currentLocation);
            size--;
            while (newMarkedPoints.Count() != 0) {
                Pos pos = newMarkedPoints.Dequeue();
                int basecost = pointValues[pos.x][pos.y];
                int x;
                int y;
                x = pos.x;
                y = pos.y + 1;
                if(x<= size && x >=0 && y <= size && y >=0) {
                    if (target.x == x && target.y == y) {
                        pathData.distance = tilePrice(board[x][y]) + basecost + 1;
                        break;
                    }
                    if (tilePrice(board[x][y]) != -1 && pointValues[x][y] == 0) {
                        pointValues[x][y] = tilePrice(board[x][y]) + basecost + 1;
                        newMarkedPoints.Enqueue(new Pos(x, y));
                    }
                }
                x = pos.x + 1;
                y = pos.y;
                if(x<= size && x >=0 && y <= size && y >=0) {
                    if (target.x == x && target.y == y) {
                        pathData.distance = tilePrice(board[x][y]) + basecost + 1;
                        break;
                    }
                    if (tilePrice(board[x][y]) != -1 && pointValues[x][y] == 0) {
                        pointValues[x][y] = tilePrice(board[x][y]) + basecost + 1;
                        newMarkedPoints.Enqueue(new Pos(x, y));
                    }
                }
                x = pos.x - 1;
                y = pos.y;
                if(x<= size && x >=0 && y <= size && y >=0) {
                    if (target.x == x && target.y == y) {
                        pathData.distance = tilePrice(board[x][y]) + basecost + 1;
                        break;
                    }
                    if (tilePrice(board[x][y]) != -1 && pointValues[x][y] == 0) {
                        pointValues[x][y] = tilePrice(board[x][y]) + basecost + 1;
                        newMarkedPoints.Enqueue(new Pos(x, y));
                    }
                }
                x = pos.x;
                y = pos.y - 1;
                if(x<= size && x >=0 && y <= size && y >=0) {
                    if (target.x == x && target.y == y) {
                        pathData.distance = tilePrice(board[x][y]) + basecost + 1;
                        break;
                    }
                    if (tilePrice(board[x][y]) != -1 && pointValues[x][y] == 0) {
                        pointValues[x][y] = tilePrice(board[x][y]) + basecost + 1;
                        newMarkedPoints.Enqueue(new Pos(x, y));
                    }
                }
            }
            //Console.Out.WriteLine(resultCost);

            //backtrace
            Pos currentPos = target;
            while (true) {
                //Console.Out.WriteLine(currentPos.x + "," + currentPos.y);
                int x, y, a=99999,b=99999,c=99999,d=99999;
                
                x = currentPos.x - 1;
                y = currentPos.y;
                if (x <= size && x >= 0 && y <= size && y >= 0) {
                    if (currentLocation.x == x && currentLocation.y == y) {
                        pathData.nextDirection = Direction.North;
                        break;
                    }
                    a = pointValues[x][y];

                }
                    x = currentPos.x;
                    y = currentPos.y - 1;
                if (x <= size && x >= 0 && y <= size && y >= 0) {
                    if (currentLocation.x == x && currentLocation.y == y) {
                        pathData.nextDirection = Direction.East;
                        break;
                    }
                    b = pointValues[x][y];
                }

                x = currentPos.x;
                y = currentPos.y + 1;
                    if (x <= size && x >= 0 && y <= size && y >= 0) {
                    if (currentLocation.x == x && currentLocation.y == y) {
                        pathData.nextDirection = Direction.West;
                        break;
                    }
                    c = pointValues[x][y];
                }

                x = currentPos.x + 1;
                y = currentPos.y;
                if (x <= size && x >= 0 && y <= size && y >= 0) {
                    if (currentLocation.x == x && currentLocation.y == y) {
                        pathData.nextDirection = Direction.South;
                        break;
                    }
                    d = pointValues[x][y];
                }
                if (a == 0) { a = 10000; }
                if (b == 0) { b = 10000; }
                if (c == 0) { c = 10000; }
                if (d == 0) { d = 10000; }
                try {
                    if (a <= b && a <= c && a <= d) {
                        pointValues[currentPos.x - 1][currentPos.y] = 9999999;
                        currentPos = new Pos(currentPos.x - 1, currentPos.y);
                    } else if (b <= a && b <= c && b <= d) {
                        pointValues[currentPos.x][currentPos.y - 1] = 9999999;
                        currentPos = new Pos(currentPos.x, currentPos.y - 1);
                    } else if (c <= b && c <= a && c <= d) {
                        pointValues[currentPos.x][currentPos.y + 1] = 9999999;
                        currentPos = new Pos(currentPos.x, currentPos.y + 1);
                    } else if (d <= b && d <= c && d <= a) {
                        pointValues[currentPos.x + 1][currentPos.y] = 9999999;
                        currentPos = new Pos(currentPos.x + 1, currentPos.y);
                    }
                } catch (Exception) {
                    Console.Out.WriteLine("Couldn't find path and had to abandon.");
                    return pathData;
                } 
            }
            Console.Out.WriteLine("Move to " + pathData.nextDirection + " with cost " + pathData.distance + " to go to" + target.x + "," + target.y);
            return pathData;
        }
        private static int tilePrice(Tile tile)
        {
            switch (tile) {
                case Tile.FREE:
                case Tile.TAVERN:
                    return 0;
                case Tile.SPIKES:
                    return 5;
                case Tile.GOLD_MINE_1:
                case Tile.GOLD_MINE_2:
                case Tile.GOLD_MINE_3:
                case Tile.GOLD_MINE_4:
                case Tile.GOLD_MINE_NEUTRAL:
                case Tile.IMPASSABLE_WOOD:
                case Tile.HERO_1:
                case Tile.HERO_2:
                case Tile.HERO_3:
                case Tile.HERO_4:
                    return -1;}
            return -1;
        }
    }


}
