using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class BaseParser
{
    private static String VARIABLE_STATEMENT_REG_EX = "^\\s*\\w+\\s+=\\s+\\w+\\s+[*+-\\\\%]\\s+\\w+\\s*$";
    private static String CONDITION_STATEMENT_REG_EX = "^\\s*if\\s+\\w+\\s+(>|<|==|!=|>=|<=)\\s+\\w+\\s+goto\\s+\\w+\\s*$";
    private static String SPACES_REG_EX = "\\s+";
    private static String NUMBERS_REG_EX = "\\d+";
    private static String NEW_LINE_REG_EX = "(\\n|\\n\\r)";
    private static String LABEL_STATEMENT_REG_EX = "^\\s*\\w+:\\s*$";
    private static String PRINT_STATEMENT_REG_EX = "^\\s*print\\s+\\w+\\s*$";
    private static String READ_STATEMENT_REG_EX = "^\\s*read\\s+\\w+\\s*$";
    private static String GOTO_STATEMENT_REG_EX = "^\\s*goto\\s+\\w+\\b\\s*$";
    private static String VARIABLE_ASSIGNMENT_STATEMENT_REG_EX = "^\\s*\\w+\\s+=\\s+\\w+\\s*$";


    private List<String> statementList;
    private StreamReader fileScanner;
    private StreamReader inputScanner;

    private Dictionary<String, int> subStringDictionary;

    private int currentStatement;

    public BaseParser(StreamReader fileScanner)
    {
        this.fileScanner = fileScanner;
        Console.WriteLine("repeat please...");
        inputScanner = new StreamReader(Console.ReadLine());
        statementList = new List<String>();
        subStringDictionary = new Dictionary<String, int>();
    }

    public void addStatements()
    {
        string line = fileScanner.ReadToEnd();

        String nextCommand = "";
        string[] result = Regex.Split(line, NEW_LINE_REG_EX);

        for (int i = 0; i < result.Length; i++)
        {

            nextCommand = result[i].ToString().Trim().ToLower();
            if (nextCommand.Equals(""))
            {
                continue;
            }
            if (nextCommand.Contains(":") && nextCommand.Split(":").Length > 1)
            {
                String[] parts = nextCommand.Split(":");
                statementList.Add((parts[0] + ":").Trim());
                statementList.Add(parts[1].Trim());
                continue;
            }
            statementList.Add(nextCommand);
        }
    }

    public void executeScript()
    {
        for (int i = 0; i < statementList.Count; i++)
        {
            if (statementList[i] == "")
            {
                statementList.Remove(statementList[i]);
            }
        }

        while (currentStatement < statementList.Count)
        {
            parseStatementType(statementList[currentStatement]);
        }
        Console.WriteLine("end");
        Console.ReadKey();
    }

    private void parseStatementType(String statement)
    {
        if (Regex.IsMatch(statement, CONDITION_STATEMENT_REG_EX))
        {
            executeConditionalStatement(statement);
            return;
        }
        if (Regex.IsMatch(statement, PRINT_STATEMENT_REG_EX))
        {
            printVarNum(statement);
            return;
        }
        if (Regex.IsMatch(statement, GOTO_STATEMENT_REG_EX))
        {
            goToLabel(statement);
            return;
        }
        if (Regex.IsMatch(statement, READ_STATEMENT_REG_EX))
        {
            readVariableValue(statement);
            return;
        }
        if (Regex.IsMatch(statement, VARIABLE_ASSIGNMENT_STATEMENT_REG_EX))
        {
            assignVariableValue(statement);
            return;
        }
        if (Regex.IsMatch(statement, VARIABLE_STATEMENT_REG_EX))
        {
            assignVariableFromExpression(statement);
            return;
        }
        if (Regex.IsMatch(statement, LABEL_STATEMENT_REG_EX))
        {
            currentStatement++;
            return;
        }
    }
    private void assignVariableFromExpression(String statement)
    {
        String[] parts = Regex.Split(statement, SPACES_REG_EX);
        int firstOperand = getOperandValue(parts[2]);
        int secondOperand = getOperandValue(parts[4]);
        int result = 0;
        switch (parts[3])
        {
            case "-":
                result = firstOperand - secondOperand;
                break;
            case "+":
                result = firstOperand + secondOperand;
                break;
            case "*":
                result = firstOperand * secondOperand;
                break;
            case "/":
                result = firstOperand / secondOperand;
                break;
            case "%":
                result = firstOperand % secondOperand;
                break;
            default:
                break;
        }

        subStringDictionary[parts[0]] = result;
        currentStatement++;
    }

    private void goToLabel(String statement)
    {
        String[] parts = Regex.Split(statement, SPACES_REG_EX);
        if (statementList.Contains(parts[1] + ":"))
        {
            currentStatement = statementList.IndexOf(parts[1] + ":");
        }
    }

    private void assignVariableValue(String statement)
    {
        String[] parts = Regex.Split(statement, SPACES_REG_EX);
        subStringDictionary[parts[0]] = getOperandValue(parts[2]);

        currentStatement++;
    }

    private void readVariableValue(String statement)
    {
        String[] parts = Regex.Split(statement, SPACES_REG_EX);
        Console.WriteLine(parts[1] + "=");

        while (Char.IsDigit(Convert.ToChar(inputScanner.Read())))
        {
            inputScanner.Read();
            Console.WriteLine("enter the number:");
            Console.WriteLine(parts[1] + "=");
        }

        int value = Convert.ToInt32(Console.ReadLine());
        subStringDictionary.Add(parts[1], value);
        currentStatement++;
    }

    private void printVarNum(String statement)
    {
        String[] parts = Regex.Split(statement, SPACES_REG_EX);
        Console.WriteLine(getOperandValue(parts[1]));
        currentStatement++;
    }

    private void executeConditionalStatement(String statement)
    {
        String[] parts = Regex.Split(statement, SPACES_REG_EX);
        bool conditionTrue = checkCondition(parts);
        if (conditionTrue)
        {
            if (statementList.Contains(parts[5] + ":"))
            {
                currentStatement = statementList.IndexOf(parts[5] + ":");
            }
        }
        else
        {
            currentStatement++;
        }
    }

    private bool checkCondition(String[] parts)
    {
        int firstOperand = getOperandValue(parts[1]);
        int secondOperand = getOperandValue(parts[3]);
        switch (parts[2])
        {
            case "==":
                return firstOperand == secondOperand;
            case "!=":
                return firstOperand != secondOperand;
            case "<":
                return firstOperand < secondOperand;
            case ">":
                return firstOperand > secondOperand;
            case ">=":
                return firstOperand >= secondOperand;
            case "<=":
                return firstOperand <= secondOperand;
            default:
                break;
        }
        return false;
    }

    private int getOperandValue(String varNum)
    {
        if (Regex.IsMatch(varNum, NUMBERS_REG_EX))
        {
            return Convert.ToInt32(varNum);
        }
        else
        {
            if (subStringDictionary[varNum] == 0)
            {
                Console.WriteLine("error in line " + (currentStatement + 1) + " variable '" + varNum + "' is not defined");
            }
            return subStringDictionary[varNum];
        }
    }

}
