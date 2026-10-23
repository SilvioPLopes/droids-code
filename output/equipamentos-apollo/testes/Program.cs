using DroidsCode.DroidCore;
if(typeof(DroidPart).GetProperty("IdItem")==null) throw new Exception("FALHA ESPERADA: peça equipada perde ID do item.");
if(typeof(DroidSalvo).GetField("pecas")==null) throw new Exception("FALHA ESPERADA: save não contém peças.");
Console.WriteLine("OK: contrato de identidade e persistência presente.");
