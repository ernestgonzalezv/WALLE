namespace Geo_Wall_E
{
    public class Scope
    {
        private Stack<Dictionary<string, Type>> Scopes = new();
        public Dictionary<string, Dictionary<int, FunctionStatement>> Variables = new();
        public Scope()
        {
            Scopes.Push(new());
            Variables = new();
        }
        public void SetScope()
        {
            Scopes.Push(new());
        }
        public Type GetTypes(string id) 
        {
            foreach (var scope in Scopes)
            {
                if (scope.TryGetValue(id, out Type? value))
                {
                    return value;
                }
            }
            throw new SemanticError(0, 0, "Variable " + id + " hasn't been declared");
        }
        public void DeleteScope()
        {
            Scopes.Pop();
        }
        public void SetSyntaxKind(string id, Type value)
        {
            Scopes.Peek()[id] = value;
        }
        public Dictionary<string, Type> GetDictionary()
        {
            return Scopes.Peek();
        }
        public void SetDictionary(Dictionary<string, Type> dict)
        {
            Scopes.Push(dict);
        }
        public void Search(FunctionStatement function)
        {
            var variables = 0;
            if (function.Args == null!) variables = -20;
            else{variables = function.Args.Count();}

            //already existssssss
            if (Variables.ContainsKey(function.Name.Lexeme!))
            {
                Dictionary<int, FunctionStatement> function1 = Variables[function.Name.Lexeme!];
                if (function1.ContainsKey(variables)) throw new SyntaxError(function.Name.Line, function.Name.Column, "Can't overwrite function");
                if (Variables[function.Name.Lexeme!].ContainsKey(-20)) Variables[function.Name.Lexeme!].Remove(-20);
                Variables[function.Name.Lexeme!].Add(variables, function);
            }
            //create function
            else
            {
                Dictionary<int, FunctionStatement> function1 = new() {{ variables, function }};
                Variables.Add(function.Name.Lexeme!, function1);
            }
        }
    }
}
