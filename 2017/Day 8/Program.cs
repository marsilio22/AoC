using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day_8
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadLines("./input.txt");

            var maxValueEver = 0;

            Dictionary<string, int> registers = new Dictionary<string, int>();

            foreach(var line in lines)
            {
                var splitLine = line.Split(' ');
                var targetReg = splitLine[0];
                var instruction = splitLine[1];
                var value = int.Parse(splitLine[2]);
                var ifStr = splitLine[3]; // this might not be used yet
                var operatorReg = splitLine[4];
                var operation = splitLine[5];
                var operationQueryValue = int.Parse(splitLine[6]);

                if (!registers.ContainsKey(targetReg)){
                    registers.Add(targetReg, 0);
                }
                if (!registers.ContainsKey(operatorReg)){
                    registers.Add(operatorReg, 0);
                }

                switch(operation){
                    // might actually be able to get the compiler to read these as is, but effort
                    // orrrr build predicates?
                    case "<":
                        if (registers[operatorReg] < operationQueryValue)
                        {
                            if (instruction.Equals("inc")) {
                                registers[targetReg] = registers[targetReg] + value;
                            }
                            else {
                                registers[targetReg] = registers[targetReg] - value;
                            }
                        }
                        break;
                    case ">":
                        if (registers[operatorReg] > operationQueryValue)
                        {
                            if (instruction.Equals("inc")) {
                                registers[targetReg] = registers[targetReg] + value;
                            }
                            else {
                                registers[targetReg] = registers[targetReg] - value;
                            }
                        }
                        break;
                    case "<=":
                        if (registers[operatorReg] <= operationQueryValue)
                        {
                            if (instruction.Equals("inc")) {
                                registers[targetReg] = registers[targetReg] + value;
                            }
                            else {
                                registers[targetReg] = registers[targetReg] - value;
                            }
                        }
                        break;
                    case ">=":
                        if (registers[operatorReg] >= operationQueryValue)
                        {
                            if (instruction.Equals("inc")) {
                                registers[targetReg] = registers[targetReg] + value;
                            }
                            else {
                                registers[targetReg] = registers[targetReg] - value;
                            }
                        }
                        break;
                    case "!=":
                        if (registers[operatorReg] != operationQueryValue)
                        {
                            if (instruction.Equals("inc")) {
                                registers[targetReg] = registers[targetReg] + value;
                            }
                            else {
                                registers[targetReg] = registers[targetReg] - value;
                            }
                        }
                        break;
                    case "==":
                        if (registers[operatorReg] == operationQueryValue)
                        {
                            if (instruction.Equals("inc")) {
                                registers[targetReg] = registers[targetReg] + value;
                            }
                            else {
                                registers[targetReg] = registers[targetReg] - value;
                            }
                        }
                        break;
                    default:
                        throw new Exception($"Missing operator {operation}");
                }

                if (maxValueEver < registers.Values.Max()){
                    maxValueEver = registers.Values.Max();
                }
            }

            Console.WriteLine(registers.Values.Max());

            Console.WriteLine(maxValueEver);
        }
    }
}
