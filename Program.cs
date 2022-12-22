using System;
using System.Linq;
using System.Collections.Generic;
using static Program_2.Program.DNA;

namespace Program_2
{
    public class Program
    {

        public class DNA
        {

            public (Professor, Room, Time)[] Genes { get; private set; }

            private static Random random = new Random();


            static int professorCount = Enum.GetNames(typeof(Professor)).Length;


            static int roomCount = Enum.GetNames(typeof(Room)).Length;


            static int timeCount = Enum.GetNames(typeof(Time)).Length;

            public DNA(int size)
            {
                Genes = new (Professor, Room, Time)[size];
                for (int i = 0; i < Genes.Length; i++)
                {
                    Genes[i] = ((Professor)random.Next(professorCount), (Room)random.Next(roomCount), (Time)random.Next(timeCount));
                }

            }

            public DNA(DNA parent1DNA, DNA parent2DNA)
            {
                int parent1 = parent1DNA.Genes.Length;
                int parent2 = parent2DNA.Genes.Length;
                Random rand = new Random();
                int dividingLine = rand.Next(parent1 + 1);
                Genes = new (Professor, Room, Time)[parent1];
                for (int i = 0; i < Genes.Length; i++)
                {
                    if (i < dividingLine)
                    {
                        Genes[i] = parent1DNA.Genes[i];
                    }
                    if (i >= dividingLine)
                    {
                        Genes[i] = parent2DNA.Genes[i];
                    }
                }
                for (int i = 0; i < Genes.Length; i++)
                {
                    int mutation = rand.Next(0, 400);
                    if (mutation == 0)
                    {
                        int professorCount = Enum.GetNames(typeof(Professor)).Length;
                        Professor randProfessor = (Professor)random.Next(professorCount);


                        int roomCount = Enum.GetNames(typeof(Room)).Length;
                        Room randRoom = (Room)random.Next(roomCount);


                        int timeCount = Enum.GetNames(typeof(Time)).Length;
                        Time randTime = (Time)random.Next(timeCount);
                        Genes[i] = ((Professor)random.Next(professorCount), (Room)random.Next(roomCount), (Time)random.Next(timeCount));
                    }
                }


            }

            public Class[] classes = new Class[]
                {
              new Class(50, "CS101A", new Professor[]{Professor.Gladbach, Professor.Gharibi, Professor.Hare, Professor.Zein_el_Din},new Professor[]{Professor.Zaman, Professor.Nait_Abdesselam}),
              new Class(50, "CS101B", new Professor[]{Professor.Gladbach, Professor.Gharibi, Professor.Hare, Professor.Zein_el_Din},new Professor[]{Professor.Zaman, Professor.Nait_Abdesselam}),
              new Class(50, "CS191A", new Professor[]{Professor.Gladbach, Professor.Gharibi, Professor.Hare, Professor.Zein_el_Din},new Professor[]{Professor.Zaman, Professor.Nait_Abdesselam}),
              new Class(50, "CS191B", new Professor[]{Professor.Gladbach, Professor.Gharibi, Professor.Hare, Professor.Zein_el_Din},new Professor[]{Professor.Zaman, Professor.Nait_Abdesselam}),
              new Class(50, "CS201", new Professor[]{Professor.Gladbach, Professor.Gharibi, Professor.Zein_el_Din, Professor.Shah},new Professor[]{Professor.Zaman, Professor.Nait_Abdesselam, Professor.Song}),
              new Class(50, "CS291", new Professor[]{Professor.Gladbach, Professor.Gharibi, Professor.Zein_el_Din, Professor.Shah},new Professor[]{Professor.Zaman, Professor.Nait_Abdesselam, Professor.Shah, Professor.Xu}),
              new Class(60, "CS303", new Professor[]{Professor.Gladbach, Professor.Hare, Professor.Zein_el_Din},new Professor[]{Professor.Zaman, Professor.Song, Professor.Shah}),
              new Class(25, "CS304", new Professor[]{Professor.Gladbach, Professor.Hare, Professor.Xu},new Professor[]{Professor.Zaman, Professor.Song, Professor.Shah, Professor.Nait_Abdesselam, Professor.Uddin, Professor.Zein_el_Din}),
              new Class(20, "CS394", new Professor[]{Professor.Xu, Professor.Song},new Professor[]{Professor.Nait_Abdesselam, Professor.Zein_el_Din}),
              new Class(60, "CS449", new Professor[]{Professor.Xu, Professor.Song, Professor.Shah},new Professor[]{Professor.Zein_el_Din, Professor.Uddin}),
              new Class(100, "CS451", new Professor[]{Professor.Xu, Professor.Song, Professor.Shah},new Professor[]{Professor.Zein_el_Din, Professor.Uddin, Professor.Nait_Abdesselam, Professor.Hare})
                };

            int[] roomCapacity = new int[]
{
                45,
                40,
                75,
                50,
                108,
                60,
                75,
                450,
                60
};

            public double Fitness()
            {
                double fitness = 0;

                //expected enrollment vs capacity
                Room[] katzOrBloch = new Room[] { Room.Bloch_119, Room.Katz_003 };

                // Same time, same room
                foreach (Room room in Enum.GetValues(typeof(Room)))
                {
                    foreach (Time time in Enum.GetValues(typeof(Time)))
                    {
                        int conflict = Genes.Where(x => x.Item2 == room && x.Item3 == time).Count();
                        if (conflict > 1) fitness -= 0.5 * conflict;
                    }
                }

                for (int i = 0; i < classes.Length; i++)
                {
                    // Capacity of the room
                    double capacity = roomCapacity[(int)Genes[i].Item2] / classes[i].size;
                    if (capacity < 1) fitness -= 0.5;
                    else if (capacity > 6) fitness -= 0.4;
                    else if (capacity > 3) fitness -= 0.2;
                    else fitness += 0.3;

                    // Preferred instructor
                    if (Array.IndexOf(classes[i].preferred, Genes[i].Item1) != -1) fitness += 0.5;
                    else if (Array.IndexOf(classes[i].other, Genes[i].Item1) != -1) fitness += 0.2;
                    else fitness -= 0.1;
                }

                foreach (Professor professor in Enum.GetValues(typeof(Professor)))
                {
                    // Courses given at the same time
                    foreach (Time time in Enum.GetValues(typeof(Room)))
                    {
                        int conflict = Genes.Where(x => x.Item1 == professor && x.Item3 == time).Count();
                        if (conflict == 1) fitness += 0.2;
                        else if (conflict > 1) fitness -= 0.2;
                    }

                    // Total number of courses given
                    (Professor, Room, Time)[] taught = Genes.Where(x => x.Item1 == professor).ToArray();
                    int total = taught.Count();
                    if (total > 4) fitness -= 0.5;
                    else if (professor != Professor.Xu && (total == 1 || total == 2)) fitness -= 0.4;

                    foreach ((Professor, Room, Time) course in taught)
                    {
                        foreach ((Professor, Room, Time) nextCourse in taught.Where(x => x.Item3 == course.Item3 + 1))
                        {
                            if (new (Professor, Room, Time)[] { course, nextCourse }.Where(x => katzOrBloch.Contains(x.Item2)).Count() == 1) fitness -= 0.4;
                            else fitness += 0.5;
                        }
                    }
                }

                // 101 & 191
                (Professor, Room, Time) CS101a = Genes[0];
                (Professor, Room, Time) CS101b = Genes[1];
                (Professor, Room, Time) CS191a = Genes[2];
                (Professor, Room, Time) CS191b = Genes[3];

                int diff101 = Math.Abs(CS101a.Item3 - CS101b.Item3);
                if (diff101 > 4) fitness += 0.5;
                else if (diff101 == 0) fitness -= 0.5;

                int diff191 = Math.Abs(CS191a.Item3 - CS191b.Item3);
                if (diff191 > 4) fitness += 0.5;
                else if (diff191 == 0) fitness -= 0.5;

                foreach ((Professor, Room, Time) CS101 in new (Professor, Room, Time)[] { CS101a, CS101b })
                {
                    foreach ((Professor, Room, Time) CS191 in new (Professor, Room, Time)[] { CS191a, CS191b })
                    {
                        int diff = Math.Abs(CS101.Item3 - CS191.Item3);
                        if (diff == 2) fitness += 0.25;
                        else if (diff == 0) fitness -= 0.25;
                        else if (diff == 1)
                        {
                            if (new (Professor, Room, Time)[] { CS101, CS191 }.Where(x => katzOrBloch.Contains(x.Item2)).Count() == 1) fitness -= 0.4;
                            else fitness += 0.5;
                        }
                    }
                }

                return fitness;
            }

            public static double[] Softmax(double[] fitnessArray)
            {
                double[] fitness_exp = fitnessArray.Select(Math.Exp).ToArray();
                double sum_fitness_exp = fitness_exp.Sum();
                double[] softmax = fitness_exp.Select(i => i / sum_fitness_exp).ToArray();
                for (int i = 0; i < softmax.ToArray().Length - 1; i++)
                {
                    softmax[i + 1] += softmax[i];
                }
                return softmax;
            }


            public enum Room
            {
                Katz_003,
                FH_216,
                Royall_206,
                Royall_201,
                FH_310,
                Haag_201,
                Haag_301,
                MNLC_325,
                Bloch_119
            }

            public enum Professor
            {
                Gharibi,
                Gladbach,
                Hare,
                Nait_Abdesselam,
                Shah,
                Song,
                Uddin,
                Xu,
                Zaman,
                Zein_el_Din
            }

            public enum Time
            {
                TenAM,
                ElevenAM,
                TwelvePM,
                OnePM,
                TwoPM,
                ThreePM
            }

            public enum Course
            {
                CS101A,
                CS101B,
                CS191A,
                CS191B,
                CS201,
                CS291,
                CS303,
                CS304,
                CS394,
                CS449,
                CS451
            }
        }

        public class Class
        {
            public int size;
            public string name;
            public DNA.Professor[] preferred;
            public DNA.Professor[] other;
            public Class(int size, string name, DNA.Professor[] preferred, DNA.Professor[] other)
            {
                this.size = size;
                this.name = name;
                this.preferred = preferred;
                this.other = other;

            }
        }
        static void Main(string[] args)
        {
            DNA[] DNAArray = Enumerable.Repeat(0, 500).Select(i => new DNA(Enum.GetNames(typeof(DNA.Course)).Length)).ToArray();
            double[] fitnessArray = new double[DNAArray.Length];
            for (int i = 0; i < DNAArray.Length; i++)
            {
                fitnessArray[i] = DNAArray[i].Fitness();
            }
            double oldAvg = Queryable.Average(fitnessArray.AsQueryable());
            double[] softmax = DNA.Softmax(fitnessArray);
            DNA[] children = new DNA[500];
            Random rand = new Random();
            bool finished = false;
            int counter = 0;
            List<double> generationalFitnesses = new List<double>();
            while (!finished)
            {
                for (int i = 0; i < children.Length; i++)
                {
                    double parent1 = rand.NextDouble();
                    double parent2 = rand.NextDouble();
                    DNA parent1DNA = DNAArray[0];
                    DNA parent2DNA = DNAArray[0];
                    for (int j = 0; j < softmax.ToArray().Length; j++)
                    {
                        if (softmax[j] >= parent1)
                        {
                            parent1DNA = DNAArray[j];
                            break;
                        }

                    }
                    for (int j = 0; j < softmax.ToArray().Length; j++)
                    {
                        if (softmax[j] >= parent2)
                        {
                            parent2DNA = DNAArray[j];
                            break;
                        }

                    }
                    children[i] = new DNA(parent1DNA, parent2DNA);
                }
                DNAArray = children;
                for (int i = 0; i < DNAArray.Length; i++)
                {
                    fitnessArray[i] = DNAArray[i].Fitness();
                }
                softmax = DNA.Softmax(fitnessArray);
                double avg = Queryable.Average(fitnessArray.AsQueryable());
                double maxValue = fitnessArray.Max();
                DNA bestSchedule = DNAArray[0];

                generationalFitnesses.Add(avg);
                if (counter >= 100 && generationalFitnesses[counter] < generationalFitnesses[counter - 100] * 1.01)
                {
                    finished = true;
                    bestSchedule = DNAArray[Array.IndexOf(fitnessArray, maxValue)];
                    for (int c = 0; c < bestSchedule.Genes.Length; c++)
                        Console.WriteLine("Course {0}: Professor {1}, Room {2}, Time {3}.", bestSchedule.classes[c].name, bestSchedule.Genes[c].Item1, bestSchedule.Genes[c].Item2, bestSchedule.Genes[c].Item3);
                    Console.WriteLine("Generations " + counter);
                    Console.WriteLine("Max Fitness " + maxValue);
                    Console.ReadLine();
                }
                counter++;

            }

        }

    }
}
