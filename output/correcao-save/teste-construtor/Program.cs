using System.Threading.Tasks;
var servico = await Task.Run(() => new SalvamentoJson());
if (servico == null) throw new Exception("Construtor retornou nulo.");
var explicito = new SalvamentoJson("arquivo-inexistente-fixture.json");
if (explicito.ExisteSave()) throw new Exception("Fixture inexistente foi encontrada.");
Console.WriteLine("OK: construção padrão em thread secundário sem API Unity; caminho explícito preservado.");
