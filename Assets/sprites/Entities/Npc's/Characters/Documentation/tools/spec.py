C,A,S,P='CANÔNICO','APROXIMAÇÃO','SUGESTÃO','PENDENTE'
SPEC={
'lip':dict(tier='Protagonista',idade='20 anos (IV.1)',funcao='Protagonista; o garoto que conserta coisas',local='Todo o Ato 1 (Ferrovale, Cratera, Estação 7)',rows=[
 ('Idade 20 anos','IV.1','Corpo "male" adulto, cabeça Human Male',C,'Idade explícita; "garoto" no texto é tratamento, não idade'),
 ('Usa botas','S3.1 ("calçar a bota")','Basic Boots (marrom)',C,'Única peça de roupa citada para ele'),
 ('Pele, cabelo, olhos','não definidos','Pele amber, Messy 1 preto, olhos castanhos',S,'Decisão de design'),
 ('Camisa e calça','não definidas','Longsleeve slate + Pants charcoal',S,'Tons desaturados do universo do jogo'),
 ('Mochila (caderno aparece na boca dela)','S6.3, S7c','Variante com mochila: Backpack + Straps (couro)',C,'Sem idle/run/combat_idle no gerador; por isso é variante separada'),
 ('Cutuca teclado com dois dedos; encolhe os ombros; fecha a mão','IV.1, S1.1','—',P,'Gestos exigem animação própria')],
 pend=['Gestos característicos (animação custom).','Variante com mochila não tem idle, run nem combat_idle.']),
'lip_mochila':dict(tier='Variante',idade='20 anos',funcao='Lip a partir da Sessão 6',local='Sessões 6–7',rows=[('Mochila','S6.3, S7c','Backpack + Backpack Straps (couro)',C,'Mesmo visual do Lip base, com mochila')],pend=['Sem idle, run e combat_idle (limitação das peças de mochila).']),
'apollo':dict(tier='Companheiro (droid)',idade='7 anos de convivência; idade real desconhecida (IV.2)',funcao='Mentor/companheiro; mediador pedagógico',local='Todo o Ato 1',rows=[
 ('Corpo robótico de peças de procedências diferentes; placas de três cores','IV.2, S1.1','Placa de torso branca + mangas amarelas + pernas/botas escuras',A,'O gerador não tem corpo robótico; zonas de cor imitam o remendo'),
 ('Placa lisa do peito que não oxida','Adendo A.4','Plate armour branca',A,'Superfície lisa clara'),
 ('Dois pontos de luz azul no lugar dos olhos','IV.2','Slit Visor em azul sobre Close Helm',A,'Visor azul contínuo em vez de dois pontos'),
 ('Alto-falante no peito','IV.2','—',P,'Sem peça equivalente'),
 ('Perna esquerda com servo emperrado (manca)','IV.2, S1.5','—',P,'Gerador é simétrico; exige camada e animação custom'),
 ('Braço de carga e olho de inspeção','S2.6','—',P,'Sem peça equivalente'),
 ('Inclina a cabeça ao escutar','IV.2','—',P,'Animação custom')],
 pend=['É apenas placeholder: para a versão final, mantenha as sprites robóticas próprias do Apollo.','Falta alto-falante, perna assimétrica, braço de carga e gesto de cabeça.']),
'ivo':dict(tier='Antagonista (Ato 1)',idade='68 anos (IV.3)',funcao='Técnico da Estação 7; trai por desespero e depois confessa',local='Estação 7 (S6); casa vazia (S2)',rows=[
 ('68 anos','IV.3','Cabeça Human Male Elderly + Wrinkles',C,'Idade explícita'),
 ('"Mais magro... não fisicamente"','S6.2','Corpo male padrão',C,'O texto nega emagrecimento físico'),
 ('Macacão (na foto, 30 anos antes)','S2.3','Overalls (slate) sobre Longsleeve cinza',A,'Macacão só é citado na foto; roupa atual não é descrita'),
 ('Pele, cabelo, barba','não definidos','Pele olive, Balding cinza, 5 O\'clock Shadow cinza',S,'"Alguém mora aqui há semanas" (S6.1) sustenta a barba por fazer'),
 ('Mãos trêmulas','Parte V, S6','—',P,'Animação custom')],pend=['Mãos trêmulas.']),
'nadir':dict(tier='Antagonista',idade='41 anos (IV.5)',funcao='Intermediária da Apex',local='Portão e praça de Ferrovale (S7b)',rows=[
 ('Mulher de quarenta e poucos anos','S7b, IV.5','Corpo female, Human Female',C,'Idade explícita'),
 ('"Gente limpa"','S7b (Pipo)','Blouse branca + Formal Pants + sapatos pretos',A,'Visual limpo e formal; peças exatas não descritas'),
 ('Caderneta encadernada debaixo do braço','S7b','—',P,'Prop sem peça equivalente'),
 ('Cabelo e pele','não definidos','Bob Side Part preto; pele taupe',S,'Decisão de design'),
 ('Cordial, nunca ameaça','IV.5','Expressão Neutral',S,'Sem sorriso nem hostilidade')],pend=['Caderneta (prop).']),
'carregador':dict(tier='Antagonista (figurante)',idade='não informada',funcao='Um dos dois carregadores que acompanham Nadir',local='S7b',rows=[
 ('Dois carregadores, "gente limpa"','S7b','Roupas claras/limpas + mochila',A,'Só a existência e a limpeza são citadas'),
 ('Aparência individual','não definida','Pele olive, Buzzcut preto',S,'Modelo reaproveitável para os dois')],pend=['Segundo carregador pode ser variação de cor deste.']),
'dara':dict(tier='NPC',idade='24 anos (IV.6)',funcao='Moradora; balde vazio e depois cheio',local='Fila do poço (S2), praça (S7)',rows=[
 ('24 anos, mulher','IV.6','Corpo female, Human Female',C,'Explícito'),
 ('Balde nas duas mãos','S2.1, S7a','—',P,'Sem peça de balde; pose segurando é animação custom'),
 ('Cabelo, pele, roupa','não definidos','Ponytail preto, pele brown, Longsleeve bluegray, Pants brown',S,'Tons gastos')],pend=['Balde.']),
'rasha':dict(tier='NPC (facção Flor de Ferro)',idade='36 anos (IV.7)',funcao='Recrutadora da Flor de Ferro',local='S4, S5, S7',rows=[
 ('Casaco reforçado com placas vermelho desbotado','S4.1','Leather armour (maroon) + Legion shoulders (vinho)',A,'Casaco reforçado aproximado por armadura de couro'),
 ('Flor de pétalas afiadas costurada no ombro','S4.1, III.8','—',P,'Emblema precisa ser camada custom'),
 ('Calça','S4.2 ("limpa a mão na calça")','Pants charcoal',C,'Existência confirmada'),
 ('Cabelo e pele','não definidos','Pixie preto, pele bronze',S,'Decisão de design'),
 ('Sorri com facilidade e nunca com alegria','IV.7','Expressão Neutral',S,'Alternativa: Happy Alt')],pend=['Emblema da flor.']),
'teodoro':dict(tier='NPC (facção Lótus Branca)',idade='52 anos (IV.8)',funcao='Líder da Lótus Branca',local='S4, S5, S7',rows=[
 ('Tons neutros, quase cinza-claro','S4.1','Longsleeve V-neck branca + Trench gray + Pants gray',A,'Paleta neutra clara'),
 ('Broche discreto de lótus branca no peito','S4.1','—',P,'Emblema custom'),
 ('Cabelo grisalho e barba aparada','não definidos','Parted gray + Trimmed Beard gray',S,'Coerente com 52 anos'),
 ('Voz baixa, cansado','IV.8','Expressão Neutral',S,'')],pend=['Broche.']),
'tacio':dict(tier='NPC',idade='não informada',funcao='Vendedor de água; segredo do cobre',local='Mercado (S1, S2), sino (S3)',rows=[
 ('Contador Geiger quebrado no pescoço','S1.4, IV.9','Chain Necklace (ferro)',A,'Colar no lugar do aparelho'),
 ('Avental de vendedor','não definido','Apron leather + Shortsleeve tan',S,''),
 ('Idade, pele, cabelo','não definidos','Balding grisalho, bigode, pele brown (≈60)',S,'Filha adulta em 2060 sugere idade avançada')],pend=['Contador Geiger como objeto.']),
'bruna':dict(tier='NPC',idade='não informada',funcao='Mestra da Guilda dos Sucateiros',local='Galpão (S1, S2, S7)',rows=[
 ('Dobra metal com a mão','S2.2, IV.9','Avental inteiro + luvas + braçadeiras de ferro',A,'Sugere ofício de metalurgia'),
 ('Idade, pele, cabelo','não definidos','Ponytail 2 cinza-escuro, pele olive',S,'')],pend=[]),
'nilce':dict(tier='NPC',idade='não informada',funcao='Estalajadeira do Cano Torto',local='Estalagem',rows=[
 ('Aparência','não descrita','Blouse tan + Plain Skirt + Half Apron; cabelo grisalho em coque',S,'Tudo é decisão de design')],pend=[]),
'quirino':dict(tier='NPC',idade='não informada',funcao='Ferreiro; instala a perna do Apollo',local='Oficina (S1, S7)',rows=[
 ('Martelo batendo','S7','—',P,'Hammer do gerador usa animação oversize; não incluído'),
 ('Aparência','não descrita','Buzzcut, Handlebar Mustache, apron leather, luvas',S,'"Grosso com carinho" sugerido por bigode e luvas')],pend=['Martelo.']),
'sula':dict(tier='NPC',idade='não informada',funcao='Vendedora de peças; delata o droid',local='Esquina leste (S1, S7b)',rows=[
 ('Aparência','não descrita','Bob preto, Tunic slate, Scarf maroon',S,'Lenço dá identidade distinta da rival Tácio')],pend=[]),
'pipo':dict(tier='NPC',idade='9 anos (IV.9)',funcao='Criança que vê Nadir chegar',local='Cidade',rows=[
 ('9 anos','IV.9','Corpo child, Human Child',C,'Explícito'),
 ('Sem calçados','—','Nenhum',A,'O gerador não tem calçado infantil'),
 ('Cabelo, roupa','não definidos','Relm Short preto, camisa gray, calça brown',S,'')],pend=['Roupa infantil só tem animação de walk.']),
'onofre':dict(tier='NPC',idade='"velho"',funcao='Velho do viaduto; lembra do mundo antes',local='Viaduto',rows=[
 ('Velho','IV.9','Human Male Elderly + Wrinkles',C,'Idade apenas qualitativa'),
 ('Cabelo branco e barba','não definidos','Balding branco + Winter Beard',S,'')],pend=[]),
'homem_fila':dict(tier='NPC (figurante)',idade='não informada',funcao='Homem da fila que diz "foi o Ivo"',local='Fila do poço (S2.1)',rows=[('Aparência','não descrita','Roupa genérica sóbria',S,'Figurante')],pend=[]),
'celia':dict(tier='Referência (foto)',idade='não informada',funcao='Autora do remendo 3.5; só aparece em foto',local='Casa de Ivo (S2.3)',rows=[
 ('Cabelo preso com pedaço de fio','S2.3','Ponytail',A,'Rabo de cavalo aproxima o fio'),
 ('Macacão','S2.3','Overalls slate',C,'Citado'),
 ('Chave inglesa na mão','S2.3','—',P,'Sem peça equivalente'),
 ('Pele, cor do cabelo','não definidos','Pele light, cabelo dark_brown',S,'')],pend=['Chave inglesa.']),
}
NOT_BUILT=[
 ('Artemis','Sem corpo por decisão do Adendo (C.1)'),
 ('Inês Andrade (mãe de Lip)','Sem descrição visual; só voz e caligrafia'),
 ('Emílio "Milo" (irmão de Dara)','Só mencionado; sem descrição'),
 ('Filha do Tácio, vizinha de Dara, pai do Pipo','Só mencionados'),
 ('Droid do Ivo, Sucatas de Captura, Droids selvagens, Drone de reconhecimento, Unidade de Manutenção Bastião 4, Droid de escolta','Não humanoides ou robóticos: o gerador LPC não cobre'),
]
