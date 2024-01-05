namespace Geo_Wall_E
{
    public class Scope
    {
        public Dictionary<string, Dictionary<int, FunctionStatement>> variablesInFunction = new();
        private Stack<Dictionary<string, Type>> scopes = new();
        public Scope()
        {
            scopes.Push(new());
            variablesInFunction = new();
        }
        public void SetScope()
        {
            scopes.Push(new());
        }
        public Type GetTypes(string id) 
        {
            foreach (var scope in scopes)
            {
                if (scope.TryGetValue(id, out Type? value))
                {
                    return value;
                }
            }
            throw new SemanticError(0, 0, "La variable " + id + " no ha sido declarada");
        }
        public void DeleteScope()
        {
            scopes.Pop();
        }
        public void SetTypes(string id, Type value)
        {
            scopes.Peek()[id] = value;
        }
        public Dictionary<string, Type> GetDictionary()
        {
            return scopes.Peek();
        }
        public void SetDictionary(Dictionary<string, Type> dict)
        {
            scopes.Push(dict);
        }
        public void Search(FunctionStatement function)
        {
            SyntaxToken functionName = function.Name;
            int variables = 0;
            if (function.Arguments == null!) variables = -1;
            else
            {
                variables = function.Arguments.Count();
            }
            if (variablesInFunction.ContainsKey(functionName.Lexeme!))
            {
                Dictionary<int, FunctionStatement> cantvariables = variablesInFunction[functionName.Lexeme!];
                if (cantvariables.ContainsKey(variables)) throw new SyntaxError(function.Name.Line, function.Name.Column, "No puede emplear la palabra " + "\"" + functionName + "\"" + " para declarar una función debido a que ya existe una función con este nombre y la misma cantidad de argumentos");
                if (variablesInFunction[functionName.Lexeme!].ContainsKey(-1)) variablesInFunction[functionName.Lexeme!].Remove(-1);
                variablesInFunction[functionName.Lexeme!].Add(variables, function);
            }
            else
            {
                Dictionary<int, FunctionStatement> cantvariables = new()
                {
                    { variables, function }
                };
                variablesInFunction.Add(functionName.Lexeme!, cantvariables);
            }
        }
    }
}
