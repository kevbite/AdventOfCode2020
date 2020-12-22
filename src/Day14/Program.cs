﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

Console.WriteLine("Part1");
Console.WriteLine(ComputerPart1.Initialize(PuzzleInput.Data).Memory.Values.Sum());
Console.WriteLine("Part2");
Console.WriteLine(ComputerPart2.Initialize(PuzzleInput.Data).Memory.Values.Sum());

public record ComputerPart1()
    : Computer(new Dictionary<long, long>(), "")
{
    public static Computer Initialize(string dockingProgram)
    {
        return Initialize(dockingProgram, new ComputerPart1());
    }

    protected override Computer Write(in long address, in long value)
    {
        var newMemory = new Dictionary<long, long>(Memory);

        var andMask = Convert.ToInt64(Mask.Replace("X", "1"), 2);
        var orMask = Convert.ToInt64(Mask.Replace("X", "0"), 2);

        newMemory[address] = value & andMask | orMask;

        return this with{ Memory = newMemory };
    }
}

public record ComputerPart2()
    : Computer(new Dictionary<long, long>(), "")
{
    public static Computer Initialize(string dockingProgram)
    {
        return Initialize(dockingProgram, new ComputerPart2());
    }

    static IEnumerable<List<int>> GetCombinations(List<int> list)
    {
        var count = Math.Pow(2, list.Count);
        for (var i = 1; i <= count - 1; i++)
        {
            var output = new List<int>();
            var str = Convert.ToString(i, 2).PadLeft(list.Count, '0');
            for (var j = 0; j < str.Length; j++)
            {
                if (str[j] == '1')
                {
                    output.Add(list[j]);
                }
            }

            yield return output;
        }
    }

    protected override Computer Write(in long address, in long value)
    {
        var newMemory = new Dictionary<long, long>(Memory);

        var andMask = Convert.ToInt64(Mask.Replace("0", "1").Replace("X", "0"), 2);
        var orMask = Convert.ToInt64(Mask.Replace("X", "0"), 2);

        var baseAddress = address & andMask | orMask;
        newMemory[baseAddress] = value;

        var floatingBitsCombinations = GetCombinations(Mask.GetAllIndexesOf('X').ToList());

        foreach (var floatingBitCombination in floatingBitsCombinations)
        {
            var sb = new StringBuilder(new string('0', 36));
            foreach (var i in floatingBitCombination)
            {
                sb[i] = '1';
            }

            newMemory[baseAddress | Convert.ToInt64(sb.ToString(), 2)] = value;
        }

        return this with{ Memory = newMemory };
    }
}

public static class Ext
{
    public static IEnumerable<int> GetAllIndexesOf(this string str, char v)
    {
        var minIndex = str.IndexOf(v);
        while (minIndex != -1)
        {
            yield return minIndex;
            minIndex = str.IndexOf(v, minIndex + 1);
        }
    }
}

public abstract record Computer(IReadOnlyDictionary<long, long> Memory, string Mask)
{
    protected static Computer Initialize(string dockingProgram, Computer computer)
    {
        var instructions = ParseInstructions(dockingProgram);

        return instructions.Aggregate(computer,
            (c, instruction) => instruction.Apply(c));
    }

    private static IEnumerable<IInstruction> ParseInstructions(string dockingProgram)
    {
        var maskRegex = new Regex(@"^mask = (?<mask>[01X]+)$");
        var writeRegex = new Regex(@"^mem\[(?<address>\d+)\] = (?<value>\d+)$");

        return dockingProgram.Split(Environment.NewLine)
            .Select(x => (maskRegex.Match(x), writeRegex.Match(x)) switch
            {
                ({Success: true} match, _) => new UpdateBitmask(match.Groups["mask"].Value),
                (_, {Success: true} match) => (IInstruction) new WriteToMemoryInstruction(
                    long.Parse(match.Groups["address"].Value), long.Parse(match.Groups["value"].Value)),
                _ => throw new InvalidOperationException()
            });
    }

    protected abstract Computer Write(in long address, in long value);

    interface IInstruction
    {
        Computer Apply(Computer computer);
    };

    record WriteToMemoryInstruction(long Address, long Value) : IInstruction
    {
        public Computer Apply(Computer computer) => computer.Write(Address, Value);
    };

    record UpdateBitmask(string Mask) : IInstruction
    {
        public Computer Apply(Computer computer) => computer with {Mask = Mask};
    };
}

public static class PuzzleInput
{
    public const string Data = @"mask = 01100101XX0010111010X010X10001010111
mem[59869] = 296403277
mem[50029] = 14565
mem[52488] = 627950233
mem[48318] = 113982010
mem[22257] = 82
mem[27870] = 17795
mem[55511] = 4594118
mask = 00000X00000110111110X000XX00010XX001
mem[61743] = 13086
mem[3724] = 4029841
mem[10560] = 236422779
mem[28511] = 7957
mem[32609] = 3123
mem[3930] = 26884
mask = 0110000XX1001011111X111010X010000100
mem[42884] = 252696502
mem[18565] = 820
mem[28279] = 105604729
mem[41611] = 1567094
mem[36341] = 5551
mem[35962] = 28056
mask = X111X001010010X1111X0XX1X11X10001X00
mem[52299] = 24969
mem[41449] = 160272674
mem[34903] = 14302599
mem[35962] = 71766
mask = X11001010XX010111010XX10100000X00101
mem[44250] = 536
mem[21842] = 494315083
mem[51716] = 3417
mem[37939] = 242985
mask = 111100X1010XX011X1111100X000000010X0
mem[35845] = 48792746
mem[26794] = 1026647
mem[47814] = 92296
mem[63043] = 3100
mem[26635] = 2181
mem[40164] = 2436842
mask = 0X1111X0X10110101011X100011X01010010
mem[63001] = 36102371
mem[47565] = 2512786
mask = 010X000111X1X01X1110XX01111100101X10
mem[18403] = 512
mem[58372] = 65318068
mem[53197] = 15875
mem[58632] = 231362257
mask = 0X1010X11110101X011000X0000000111001
mem[4095] = 116134614
mem[63365] = 186252884
mem[19024] = 2449
mem[30536] = 1021
mem[49282] = 447504
mem[16658] = 98394885
mask = 0111000111001011X11011000X1111X0100X
mem[64187] = 1431
mem[11141] = 1904
mem[39855] = 61495
mask = 1111000XX100X01X111111111110XX011X11
mem[44407] = 1449308
mem[43902] = 2006
mem[62547] = 3478
mask = 0X1010010100X011101000X00010X0100111
mem[3885] = 140726549
mem[3307] = 938307640
mem[22257] = 71231
mem[41243] = 508095
mem[25091] = 24579
mask = X0000000XX011011111010X000XX0X000X01
mem[22764] = 96570583
mem[36352] = 56148675
mask = 0X0X0011X110111010X001X10XX000101010
mem[38443] = 1755
mem[56360] = 25308
mask = 11X0X00000X11X11111001XXXXX100001111
mem[7833] = 785
mem[4380] = 2874
mask = 0X1000X11X0XX0X1101001011110001X0000
mem[61544] = 144576256
mem[2156] = 1658
mem[26571] = 28977534
mem[43902] = 1032904266
mem[21524] = 463
mask = 100X000010011011111XX01010001010X101
mem[47123] = 5438
mem[28936] = 39885
mem[55045] = 946386
mem[43630] = 143495580
mem[10560] = 3231
mask = 0110000X1X00101111XXX0000000010010X1
mem[39551] = 524769
mem[37150] = 397326
mem[153] = 2696665
mem[43255] = 44
mem[14355] = 14827
mem[51242] = 445851
mask = X1X000X1110X10111X101100100X0X000XX0
mem[40969] = 756383545
mem[26794] = 100307329
mem[48938] = 1863389
mem[2345] = 2535119
mem[53880] = 1759799
mask = 11X0X0001X0110111X001X0011XX000XX000
mem[8343] = 11387
mem[48589] = 60
mem[27590] = 5929131
mem[3690] = 63744500
mem[520] = 31249543
mem[24387] = 473
mem[21573] = 32235112
mask = 11101001011X10111X10X111X00000000000
mem[50565] = 563
mem[26571] = 3496
mem[3930] = 418
mem[35770] = 7558838
mem[12614] = 1619627
mask = 011X010111001X11111001100000X00XX1X1
mem[50701] = 111977200
mem[19006] = 10909
mem[16052] = 74134
mem[43094] = 14253
mem[50557] = 608639
mem[59406] = 1422
mask = 110X00001101101X11100XX011X000000011
mem[52383] = 4751812
mem[1669] = 833448227
mem[35841] = 439768021
mem[43630] = 7511
mem[53126] = 343642
mem[60802] = 1256603
mask = 11100XXX11XX101111101100000X00X01010
mem[49511] = 4982217
mem[51824] = 270
mem[51349] = 1890
mem[59808] = 3773655
mask = 111000XX110X10X11110011010110101101X
mem[57730] = 57401
mem[35630] = 973239
mem[54672] = 138240715
mem[8136] = 10996
mem[55459] = 401980
mem[17187] = 941
mask = X111XX01X100101111X00X0001000000X011
mem[41484] = 1242838
mem[29717] = 1145160
mem[27394] = 1108224
mem[2816] = 272760856
mask = 1XX00101000010111010X1111000X0010010
mem[32201] = 304518
mem[16394] = 79826575
mem[39941] = 13157
mem[35282] = 760997
mem[63951] = 7011404
mem[8985] = 123284486
mask = 011X100111000011X110X0110000X1111010
mem[34856] = 39388801
mem[18190] = 3208452
mask = 1X000X11XX1010111X10001111X00X101X00
mem[32295] = 1729184
mem[48809] = 3708309
mem[17960] = 66227
mem[39063] = 5571972
mem[34523] = 1100265
mem[17724] = 1074235
mask = 1X000000100110X111101X1001010000010X
mem[2309] = 1178
mem[56623] = 5006
mem[41091] = 951374
mem[63942] = 21970475
mem[8120] = 1167
mem[50184] = 5634
mask = 11X0001X111111X11010100XX0101X111000
mem[63703] = 31429
mem[19083] = 32307
mem[48832] = 3825073
mask = 11X000111111X11110X00000001000011X11
mem[53491] = 953
mem[39752] = 91899271
mask = 111XX011X1011011111XX110X00X00000000
mem[63089] = 3036
mem[38445] = 356279
mem[40137] = 10955
mem[37568] = 477812
mem[18443] = 85
mask = 11X1100101X0101111X000X001X111101XX1
mem[64998] = 758355504
mem[10506] = 5946
mem[58372] = 800
mem[40606] = 1267021
mem[42753] = 86680
mem[38503] = 9164580
mask = 1X110000XX100000X100010001110000X010
mem[4805] = 898
mem[50969] = 219378
mem[3182] = 14757
mask = 01101X0111X01011X0X000000111X0101001
mem[26794] = 289
mem[27899] = 67683
mask = X1X0001111XX1X1X10X0X0111110000010X0
mem[60968] = 128881
mem[21049] = 153946
mem[4625] = 6492
mem[13554] = 14536684
mask = 1110100X1X0110111X001011X1111X0X010X
mem[49387] = 48190714
mem[5514] = 58395
mem[59861] = 2590
mem[59717] = 706
mask = 110000XX1101X01111100001111000X00000
mem[52288] = 594838
mem[47711] = 256545
mem[37150] = 5576185
mask = X1001000100X101110X0X1001X01X1X00X0X
mem[12194] = 1010012
mem[55682] = 745
mem[19810] = 54828
mask = X111100111001011X10X0110110X00000011
mem[10747] = 10766086
mem[40969] = 6443091
mem[2563] = 7520
mask = X110X001X10010111X1001X0001010X00X11
mem[16385] = 16194
mem[9178] = 1770
mem[3885] = 584370
mem[32909] = 551495
mem[21842] = 534
mem[59406] = 4042521
mask = X110X0001100101X11100X00XXX0X00000X1
mem[62127] = 1228052
mem[34922] = 165241779
mem[38187] = 7559
mask = 00X101X111X0X1X1011010001X11111X0111
mem[37035] = 51004
mem[64187] = 9284
mem[38834] = 163012800
mask = 0X1X1001110010X111101X00101101001011
mem[41856] = 13039831
mem[63376] = 1043992
mask = 00X000000X01X01111X01X011001101X11X0
mem[14490] = 10619
mem[63497] = 64
mem[8985] = 2445
mem[3372] = 2134806
mem[43902] = 25402
mem[63408] = 2150231
mem[35251] = 252
mask = 01100001010X101X11X0XX0X000010000010
mem[11427] = 40388
mem[5594] = 2064
mem[14642] = 3216356
mem[33886] = 16148
mem[22872] = 317877
mem[16905] = 22391
mem[59260] = 14964908
mask = 11X01000X10X1011110010000101X1XXX1X0
mem[40205] = 162183
mem[52774] = 21039251
mem[47529] = 13977
mask = 111X1X0101X0101111100X1011X10X0001XX
mem[50214] = 131677558
mem[37828] = 45776303
mem[25486] = 270926
mem[3307] = 100144082
mask = 0X1X0101110011X1XX1010X010001X00X110
mem[24709] = 4703889
mem[20251] = 4768780
mem[23739] = 292844
mem[33886] = 59676
mem[29424] = 157758852
mem[48117] = 434386871
mask = 111010010100X01X1X10101111XX10000X00
mem[24094] = 50839
mem[40525] = 62507
mem[16058] = 11731
mem[57853] = 286
mem[13702] = 252
mem[61517] = 92673
mem[28899] = 10302
mask = 0X101000011X10X111001X10101001011010
mem[39054] = 164757015
mem[46804] = 194909
mem[17096] = 16017
mem[50214] = 474
mem[55787] = 471712
mask = 010X0X0X110110111X1X10000X000XX011X0
mem[50969] = 44594
mem[62079] = 4008
mem[26760] = 1302
mem[40242] = 450994
mask = 00100000000010X1010011X100X10X000111
mem[13951] = 29184
mem[44387] = 733
mem[56481] = 1419987
mem[21049] = 113460142
mask = 111000X01101101X10001X01110X01000001
mem[37955] = 23023
mem[23639] = 7326
mem[41939] = 125079
mask = 010X101X00X110XXX011010100X10011100X
mem[4000] = 325975899
mem[65121] = 332644116
mem[41463] = 345
mem[16748] = 1087582
mem[37842] = 894
mask = X110100111X01011X11011XX00X0101X0001
mem[19704] = 11095
mem[8715] = 72847
mem[41939] = 6370749
mem[12294] = 6805400
mem[21842] = 79
mask = 01XX1X10XX011X1X101X01010001X1010011
mem[50017] = 55985
mem[49915] = 1470
mem[38942] = 1053875
mem[16748] = 874238254
mem[59353] = 678489
mem[57733] = 14739
mask = 111111XX01X010X11110000001X11000111X
mem[40606] = 199
mem[17692] = 1696176
mem[11913] = 4169294
mask = 01101X0X01001011101XXX10001100001010
mem[23120] = 237551058
mem[28299] = 142
mask = 1X10X0011110101X1X1X11001100110X1001
mem[6118] = 115871
mem[19568] = 514315
mem[24764] = 365
mask = 1110X0X11X0XX0X1111X11011X0000000000
mem[47355] = 25865880
mem[51774] = 163286264
mem[37432] = 2954
mem[31036] = 12067
mask = 01110X01110010111XX001100001010X1101
mem[28516] = 323191
mem[43255] = 3274
mem[58073] = 86929637
mask = 01XX0001XX11001111X0X100X1010X101000
mem[18330] = 378470
mem[39651] = 1052033
mem[7784] = 143118549
mem[32641] = 10950293
mem[4029] = 836004
mask = 0110010X0100XX1X10101010101000X01110
mem[64998] = 399249156
mem[16391] = 2391
mem[15113] = 25159
mem[35039] = 1672488
mem[44901] = 4947519
mask = 001000X11101101X1X1000X0X00000111000
mem[23194] = 192980783
mem[2379] = 26471
mem[52040] = 748413
mask = XX101X11X1X01110X1100X0011X01110111X
mem[35610] = 3487435
mem[15827] = 1447
mem[52266] = 8003180
mem[6315] = 254747938
mem[38582] = 21341903
mem[23012] = 1942
mem[52040] = 1040637609
mask = 1110X011110XX0011111X001X000000111X0
mem[19431] = 6438
mem[55404] = 381
mem[49702] = 21205234
mem[28299] = 43345
mem[32419] = 3944214
mem[15970] = 1406843
mask = 0110X001XX010X01XX1X1010111001000000
mem[63608] = 2388415
mem[19550] = 397
mem[64770] = 354
mask = 011XX011X1101X10X11001X0X0001X101001
mem[27886] = 54971772
mem[61000] = 547
mem[9281] = 5472661
mem[32039] = 20450
mem[47529] = 214406
mem[32922] = 2186075
mask = X1X0000X0100101110X00100000010000101
mem[54996] = 136156
mem[1645] = 1422
mem[645] = 51359613
mask = 010X000110X100X1110000010011110010XX
mem[58473] = 54000741
mem[50214] = 118839
mem[60719] = 4225
mask = 11101000110010101X10X11011X00010X011
mem[4967] = 2810
mem[55561] = 270767273
mem[50906] = 323094280
mem[544] = 1779102
mem[12700] = 122462
mem[20654] = 71
mask = 01100001X0X1000101X011101X100000X00X
mem[40754] = 6033263
mem[3724] = 640
mem[6776] = 274
mask = 00110X0111001111X010110000X010X00110
mem[59461] = 40987
mem[49367] = 418940480
mem[12155] = 58648
mem[50876] = 55597938
mem[39338] = 125566
mask = 01100001X110101111X001101X1100X0100X
mem[16658] = 13293
mem[19410] = 355061209
mem[30127] = 9284451
mem[35805] = 2422
mask = 1X11X0010100X01X111X1101XX1101010011
mem[10560] = 116
mem[42153] = 4817997
mem[40333] = 970832
mem[19083] = 63018397
mask = 011XX0X111XX101111100XX0101010011010
mem[51898] = 95230576
mem[49059] = 481728904
mem[25416] = 1192004
mem[6045] = 244681888
mem[22225] = 208962448
mask = X11X100XX1X0101111XX1110101X110110X0
mem[10560] = 3688
mem[50016] = 3038
mem[25234] = 52018301
mask = X11XX001X100101111100110X011110X1XXX
mem[8343] = 111304110
mem[9100] = 833307
mem[43853] = 836994
mem[9178] = 24458493
mem[59596] = 257520799
mask = 101X0X0000100X0X010000X1X0010X0XX111
mem[13597] = 887
mem[16422] = 232
mem[52384] = 120276377
mem[10834] = 6933
mem[16058] = 992102418
mem[24456] = 92155
mask = X11000010100101111X01100001X10001XX1
mem[37568] = 26930
mem[16385] = 232190606
mem[8340] = 113193119
mem[35910] = 832
mask = 11111001010010111110100XX1X11X100111
mem[56429] = 6105943
mem[33886] = 8020
mem[51774] = 168714
mask = 1111100101101X11110000000X0110001X0X
mem[37828] = 28012
mem[51551] = 320681847
mem[34281] = 52088
mask = 0110X001110XX01111100010X00X010X1010
mem[36720] = 26721
mem[39098] = 8365128
mem[28279] = 994775
mem[59762] = 1466626
mem[17088] = 910296
mem[33578] = 4789
mem[31320] = 11279
mask = X1010000X1011011101001XX000X010011X1
mem[58476] = 20790
mem[33584] = 339
mem[21127] = 471449302
mask = 0100X01X1101X011X010010XX0000X011000
mem[39915] = 1754512
mem[51774] = 183707725
mem[43094] = 2797257
mem[21120] = 8809
mask = X010000X00XXX0X1X10010X11101X0100110
mem[48113] = 54854990
mem[43108] = 127392
mem[57733] = 4840137
mem[17088] = 7272071
mem[65436] = 1211
mask = X11010X01100101010100X11111000101X01
mem[38787] = 139
mem[9667] = 37073154
mem[18731] = 370
mem[4538] = 15900
mem[13202] = 517905282
mask = X11000011100001X1110X01001100X001010
mem[12467] = 932
mem[14070] = 12280
mem[40105] = 184487874
mem[33314] = 832859
mask = 0X100000X1001011X11X1000X10001X0X000
mem[17724] = 3496777
mem[53907] = 23167
mem[63544] = 1614248
mem[50209] = 2618603
mem[36348] = 384412976
mem[44536] = 16223523
mask = X110X1011X0011X11110X111X0X00001X000
mem[4784] = 11971978
mem[10982] = 398035940
mem[7005] = 515
mem[35241] = 1093398
mem[63779] = 1906
mask = 1100X00XX001101111X0X101X1010X001011
mem[36341] = 2194817
mem[40531] = 724
mem[5658] = 468782968
mem[62005] = 1168
mem[3491] = 1329281
mem[49511] = 672
mem[49173] = 1048435
mask = 1110XX0010X1101X11X01X1010011101010X
mem[44290] = 660182
mem[21184] = 291220
mem[39098] = 2769543
mem[30987] = 24159
mask = 1010X00X00100X1111000X011XX10100X110
mem[48117] = 29838
mem[23924] = 4158
mem[46502] = 501874
mem[32481] = 713511
mask = XX1000111XX1X01110X0X1001001X000100X
mem[11848] = 60796
mem[35576] = 125075628
mem[7365] = 367051456
mem[49453] = 2467302
mem[63274] = 2446
mask = X1100X01X10010111X10X10000X00000XXX1
mem[59406] = 7522
mem[26316] = 194122
mem[33419] = 24756556
mem[17066] = 827
mask = 111100010100001011111X101011X000X0X0
mem[28511] = 54553
mem[9469] = 136199
mem[6481] = 674106955
mem[4029] = 62909806
mask = 0010000000011X0X110X11X11XX100100011
mem[19006] = 23636330
mem[49695] = 113355
mem[9478] = 25169678
mem[26475] = 7337
mem[1712] = 89775255
mem[32101] = 917
mask = 1X00X00X1101101111X010X011100000XXX0
mem[24770] = 10592648
mem[40969] = 757841
mem[42211] = 7319070
mem[42753] = 277734
mem[30310] = 11988774
mem[19470] = 89618256
mem[2816] = 89780492";
}