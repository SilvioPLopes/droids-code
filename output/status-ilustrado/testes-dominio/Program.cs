using DroidsCode.DroidCore;
static void Check(bool ok,string erro){if(!ok)throw new Exception(erro);}
var d = new Droid("Teste",100);
Check(d.EnergiaAtual==100 && d.EnergiaMaxima==100,"inicial");
var fraca = new TecnicaComposta{NivelDeDano=1};
var forte = new TecnicaComposta{NivelDeDano=3};
Check(TabelaDeEnergia.CalcularCusto(fraca)==10,"custo fraca");
Check(TabelaDeEnergia.CalcularCusto(forte)==30,"custo forte");
forte.EfeitosDeAtributo.Add(new EfeitoDeAtributo{Valor=5,DuracaoEmTurnos=2});
Check(TabelaDeEnergia.CalcularCusto(forte)==40,"custo efeito");
forte.EfeitosDeAtributo[0].Valor=15;
Check(TabelaDeEnergia.CalcularCusto(forte)==60,"custo intensidade");
forte.EfeitosDeDanoPorTurno.Add(new EfeitoDeDanoPorTurno{DanoPorTurno=5,DuracaoEmTurnos=3});
Check(TabelaDeEnergia.CalcularCusto(forte)==75,"custo veneno");
forte.NivelDeDano=int.MaxValue;Check(TabelaDeEnergia.CalcularCusto(forte)==100,"teto");
d.RestaurarEnergia(-5,100);Check(d.EnergiaAtual==0,"limite inferior");
d.RestaurarEnergia(500,100);Check(d.EnergiaAtual==100,"limite superior");
d.RestaurarEnergia(0,100);Check(d.EnergiaAtual==0,"saldo zero");
d.TecnicasConfiguradas["Fraca"]=fraca;
Check(d.ObterCustoDeEnergia("Fraca")==10 && d.ObterCustoDeEnergia(Droid.NomeAtaqueBasico)==0,"consulta de custo");
var legado = new DadosDoJogo{versao=6,droid=new DroidSalvo()};
SalvamentoJson.MigrarDados(legado);Check(legado.droid.energiaAtual==100 && legado.droid.energiaMaxima==100,"migra legado");
var novo = new DadosDoJogo{versao=6,droid=new DroidSalvo{energiaAtual=0,energiaMaxima=100}};
SalvamentoJson.MigrarDados(novo);Check(novo.droid.energiaAtual==0,"migra zero");
Console.WriteLine("OK: domínio real compilado; custo10/30, duração/intensidade/veneno40/60/75, teto100, saldo limitado e zero preservado, básico gratuito, migração DTO legado/novo. Sem Unity nativo, JSON, disco de save ou combate aleatório.");
try {
    var alvo = new Droid("Alvo",100);
    var resultado=d.ExecutarAcao("Fraca",alvo);
    Check(!resultado.Sucesso && resultado.DanoCausado==0 && d.EnergiaAtual==0 && alvo.EfeitosAtivos.Count==0,"saldo insuficiente");
    Console.WriteLine("OK: técnica recusada no domínio por saldo zero, sem dano/efeitos ou consumo.");
} catch (System.Security.SecurityException) {
    Console.WriteLine("PENDENTE: chamada de combate requer runtime nativo Unity; usar fixture no Editor.");
}
