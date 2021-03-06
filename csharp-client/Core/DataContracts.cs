﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace CoveoBlitz
{
    [DataContract]
    public class GameResponse
    {
        [DataMember]
        public Game game;

        [DataMember]
        public Hero hero;

        [DataMember]
        public string token;

        [DataMember]
        public string viewUrl;

        [DataMember]
        public string playUrl;
    }

    [DataContract]
    public class Game
    {
        [DataMember]
        public string id;

        [DataMember]
        public int turn;

        [DataMember]
        public int maxTurns;

        [DataMember]
        public List<Hero> heroes;

        [DataMember]
        public Board board;

        [DataMember]
        public bool finished;
    }

    [DataContract]
    public class Hero
    {
        [DataMember]
        public int id;

        [DataMember]
        public string name;

        [DataMember]
        public int elo;

        [DataMember]
        public Pos pos;

        [DataMember]
        public int life;

        [DataMember]
        public int gold;

        [DataMember]
        public int mineCount;

        [DataMember]
        public Pos spawnPos;

        [DataMember]
        public bool crashed;
    }

    [DataContract]
    public class Pos
    {
        [DataMember]
        public int x;

        [DataMember]
        public int y;

        public Pos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Pos()
        {
            this.x = 0;
            this.y = 0;
        }

        public static int DistanceBetween(Pos p1, Pos p2)
        {
            Debug.Assert(p1 != null);
            Debug.Assert(p2 != null);

            int xDist = p1.x < p2.x ? p2.x - p1.x : p1.x - p2.x;
            int yDist = p1.y < p2.y ? p2.y - p1.y : p1.y - p2.y;
            return xDist + yDist;
        }
    }

    [DataContract]
    public class Board
    {
        [DataMember]
        public int size;

        [DataMember]
        public string tiles;
    }
}