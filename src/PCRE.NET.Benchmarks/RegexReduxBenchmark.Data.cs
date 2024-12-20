﻿using System.Diagnostics.CodeAnalysis;

namespace PCRE.NET.Benchmarks;

/// <summary>
/// This data is from the regex-redux benchmark by the Benchmark's Game.
/// https://benchmarksgame-team.pages.debian.net/benchmarksgame/description/regexredux.html
/// </summary>
[SuppressMessage("ReSharper", "StringLiteralTypo")]
internal static class RegexReduxBenchmarkData
{
    public static readonly string[] Patterns =
    [
        "agggtaaa|tttaccct",
        "[cgt]gggtaaa|tttaccc[acg]",
        "a[act]ggtaaa|tttacc[agt]t",
        "ag[act]gtaaa|tttac[agt]ct",
        "agg[act]taaa|ttta[agt]cct",
        "aggg[acg]aaa|ttt[cgt]ccct",
        "agggt[cgt]aa|tt[acg]accct",
        "agggta[cgt]a|t[acg]taccct",
        "agggtaa[cgt]|[acg]ttaccct"
    ];

    public static readonly string Subject
        = """
          >ONE Homo sapiens alu
          GGCCGGGCGCGGTGGCTCACGCCTGTAATCCCAGCACTTTGGGAGGCCGAGGCGGGCGGA
          TCACCTGAGGTCAGGAGTTCGAGACCAGCCTGGCCAACATGGTGAAACCCCGTCTCTACT
          AAAAATACAAAAATTAGCCGGGCGTGGTGGCGCGCGCCTGTAATCCCAGCTACTCGGGAG
          GCTGAGGCAGGAGAATCGCTTGAACCCGGGAGGCGGAGGTTGCAGTGAGCCGAGATCGCG
          CCACTGCACTCCAGCCTGGGCGACAGAGCGAGACTCCGTCTCAAAAAGGCCGGGCGCGGT
          GGCTCACGCCTGTAATCCCAGCACTTTGGGAGGCCGAGGCGGGCGGATCACCTGAGGTCA
          GGAGTTCGAGACCAGCCTGGCCAACATGGTGAAACCCCGTCTCTACTAAAAATACAAAAA
          TTAGCCGGGCGTGGTGGCGCGCGCCTGTAATCCCAGCTACTCGGGAGGCTGAGGCAGGAG
          AATCGCTTGAACCCGGGAGGCGGAGGTTGCAGTGAGCCGAGATCGCGCCACTGCACTCCA
          GCCTGGGCGACAGAGCGAGACTCCGTCTCAAAAAGGCCGGGCGCGGTGGCTCACGCCTGT
          AATCCCAGCACTTTGGGAGGCCGAGGCGGGCGGATCACCTGAGGTCAGGAGTTCGAGACC
          AGCCTGGCCAACATGGTGAAACCCCGTCTCTACTAAAAATACAAAAATTAGCCGGGCGTG
          GTGGCGCGCGCCTGTAATCCCAGCTACTCGGGAGGCTGAGGCAGGAGAATCGCTTGAACC
          CGGGAGGCGGAGGTTGCAGTGAGCCGAGATCGCGCCACTGCACTCCAGCCTGGGCGACAG
          AGCGAGACTCCGTCTCAAAAAGGCCGGGCGCGGTGGCTCACGCCTGTAATCCCAGCACTT
          TGGGAGGCCGAGGCGGGCGGATCACCTGAGGTCAGGAGTTCGAGACCAGCCTGGCCAACA
          TGGTGAAACCCCGTCTCTACTAAAAATACAAAAATTAGCCGGGCGTGGTGGCGCGCGCCT
          GTAATCCCAGCTACTCGGGAGGCTGAGGCAGGAGAATCGCTTGAACCCGGGAGGCGGAGG
          TTGCAGTGAGCCGAGATCGCGCCACTGCACTCCAGCCTGGGCGACAGAGCGAGACTCCGT
          CTCAAAAAGGCCGGGCGCGGTGGCTCACGCCTGTAATCCCAGCACTTTGGGAGGCCGAGG
          CGGGCGGATCACCTGAGGTCAGGAGTTCGAGACCAGCCTGGCCAACATGGTGAAACCCCG
          TCTCTACTAAAAATACAAAAATTAGCCGGGCGTGGTGGCGCGCGCCTGTAATCCCAGCTA
          CTCGGGAGGCTGAGGCAGGAGAATCGCTTGAACCCGGGAGGCGGAGGTTGCAGTGAGCCG
          AGATCGCGCCACTGCACTCCAGCCTGGGCGACAGAGCGAGACTCCGTCTCAAAAAGGCCG
          GGCGCGGTGGCTCACGCCTGTAATCCCAGCACTTTGGGAGGCCGAGGCGGGCGGATCACC
          TGAGGTCAGGAGTTCGAGACCAGCCTGGCCAACATGGTGAAACCCCGTCTCTACTAAAAA
          TACAAAAATTAGCCGGGCGTGGTGGCGCGCGCCTGTAATCCCAGCTACTCGGGAGGCTGA
          GGCAGGAGAATCGCTTGAACCCGGGAGGCGGAGGTTGCAGTGAGCCGAGATCGCGCCACT
          GCACTCCAGCCTGGGCGACAGAGCGAGACTCCGTCTCAAAAAGGCCGGGCGCGGTGGCTC
          ACGCCTGTAATCCCAGCACTTTGGGAGGCCGAGGCGGGCGGATCACCTGAGGTCAGGAGT
          TCGAGACCAGCCTGGCCAACATGGTGAAACCCCGTCTCTACTAAAAATACAAAAATTAGC
          CGGGCGTGGTGGCGCGCGCCTGTAATCCCAGCTACTCGGGAGGCTGAGGCAGGAGAATCG
          CTTGAACCCGGGAGGCGGAGGTTGCAGTGAGCCGAGATCGCGCCACTGCACTCCAGCCTG
          GGCGACAGAGCGAGACTCCG
          >TWO IUB ambiguity codes
          cttBtatcatatgctaKggNcataaaSatgtaaaDcDRtBggDtctttataattcBgtcg
          tactDtDagcctatttSVHtHttKtgtHMaSattgWaHKHttttagacatWatgtRgaaa
          NtactMcSMtYtcMgRtacttctWBacgaaatatagScDtttgaagacacatagtVgYgt
          cattHWtMMWcStgttaggKtSgaYaaccWStcgBttgcgaMttBYatcWtgacaYcaga
          gtaBDtRacttttcWatMttDBcatWtatcttactaBgaYtcttgttttttttYaaScYa
          HgtgttNtSatcMtcVaaaStccRcctDaataataStcYtRDSaMtDttgttSagtRRca
          tttHatSttMtWgtcgtatSSagactYaaattcaMtWatttaSgYttaRgKaRtccactt
          tattRggaMcDaWaWagttttgacatgttctacaaaRaatataataaMttcgDacgaSSt
          acaStYRctVaNMtMgtaggcKatcttttattaaaaagVWaHKYagtttttatttaacct
          tacgtVtcVaattVMBcttaMtttaStgacttagattWWacVtgWYagWVRctDattBYt
          gtttaagaagattattgacVatMaacattVctgtBSgaVtgWWggaKHaatKWcBScSWa
          accRVacacaaactaccScattRatatKVtactatatttHttaagtttSKtRtacaaagt
          RDttcaaaaWgcacatWaDgtDKacgaacaattacaRNWaatHtttStgttattaaMtgt
          tgDcgtMgcatBtgcttcgcgaDWgagctgcgaggggVtaaScNatttacttaatgacag
          cccccacatYScaMgtaggtYaNgttctgaMaacNaMRaacaaacaKctacatagYWctg
          ttWaaataaaataRattagHacacaagcgKatacBttRttaagtatttccgatctHSaat
          actcNttMaagtattMtgRtgaMgcataatHcMtaBSaRattagttgatHtMttaaKagg
          YtaaBataSaVatactWtataVWgKgttaaaacagtgcgRatatacatVtHRtVYataSa
          KtWaStVcNKHKttactatccctcatgWHatWaRcttactaggatctataDtDHBttata
          aaaHgtacVtagaYttYaKcctattcttcttaataNDaaggaaaDYgcggctaaWSctBa
          aNtgctggMBaKctaMVKagBaactaWaDaMaccYVtNtaHtVWtKgRtcaaNtYaNacg
          gtttNattgVtttctgtBaWgtaattcaagtcaVWtactNggattctttaYtaaagccgc
          tcttagHVggaYtgtNcDaVagctctctKgacgtatagYcctRYHDtgBattDaaDgccK
          tcHaaStttMcctagtattgcRgWBaVatHaaaataYtgtttagMDMRtaataaggatMt
          ttctWgtNtgtgaaaaMaatatRtttMtDgHHtgtcattttcWattRSHcVagaagtacg
          ggtaKVattKYagactNaatgtttgKMMgYNtcccgSKttctaStatatNVataYHgtNa
          BKRgNacaactgatttcctttaNcgatttctctataScaHtataRagtcRVttacDSDtt
          aRtSatacHgtSKacYagttMHtWataggatgactNtatSaNctataVtttRNKtgRacc
          tttYtatgttactttttcctttaaacatacaHactMacacggtWataMtBVacRaSaatc
          cgtaBVttccagccBcttaRKtgtgcctttttRtgtcagcRttKtaaacKtaaatctcac
          aattgcaNtSBaaccgggttattaaBcKatDagttactcttcattVtttHaaggctKKga
          tacatcBggScagtVcacattttgaHaDSgHatRMaHWggtatatRgccDttcgtatcga
          aacaHtaagttaRatgaVacttagattVKtaaYttaaatcaNatccRttRRaMScNaaaD
          gttVHWgtcHaaHgacVaWtgttScactaagSgttatcttagggDtaccagWattWtRtg
          ttHWHacgattBtgVcaYatcggttgagKcWtKKcaVtgaYgWctgYggVctgtHgaNcV
          taBtWaaYatcDRaaRtSctgaHaYRttagatMatgcatttNattaDttaattgttctaa
          ccctcccctagaWBtttHtBccttagaVaatMcBHagaVcWcagBVttcBtaYMccagat
          gaaaaHctctaacgttagNWRtcggattNatcRaNHttcagtKttttgWatWttcSaNgg
          gaWtactKKMaacatKatacNattgctWtatctaVgagctatgtRaHtYcWcttagccaa
          tYttWttaWSSttaHcaaaaagVacVgtaVaRMgattaVcDactttcHHggHRtgNcctt
          tYatcatKgctcctctatVcaaaaKaaaagtatatctgMtWtaaaacaStttMtcgactt
          taSatcgDataaactaaacaagtaaVctaggaSccaatMVtaaSKNVattttgHccatca
          cBVctgcaVatVttRtactgtVcaattHgtaaattaaattttYtatattaaRSgYtgBag
          aHSBDgtagcacRHtYcBgtcacttacactaYcgctWtattgSHtSatcataaatataHt
          cgtYaaMNgBaatttaRgaMaatatttBtttaaaHHKaatctgatWatYaacttMctctt
          ttVctagctDaaagtaVaKaKRtaacBgtatccaaccactHHaagaagaaggaNaaatBW
          attccgStaMSaMatBttgcatgRSacgttVVtaaDMtcSgVatWcaSatcttttVatag
          ttactttacgatcaccNtaDVgSRcgVcgtgaacgaNtaNatatagtHtMgtHcMtagaa
          attBgtataRaaaacaYKgtRccYtatgaagtaataKgtaaMttgaaRVatgcagaKStc
          tHNaaatctBBtcttaYaBWHgtVtgacagcaRcataWctcaBcYacYgatDgtDHccta
          >THREE Homo sapiens frequency
          aacacttcaccaggtatcgtgaaggctcaagattacccagagaacctttgcaatataaga
          atatgtatgcagcattaccctaagtaattatattctttttctgactcaaagtgacaagcc
          ctagtgtatattaaatcggtatatttgggaaattcctcaaactatcctaatcaggtagcc
          atgaaagtgatcaaaaaagttcgtacttataccatacatgaattctggccaagtaaaaaa
          tagattgcgcaaaattcgtaccttaagtctctcgccaagatattaggatcctattactca
          tatcgtgtttttctttattgccgccatccccggagtatctcacccatccttctcttaaag
          gcctaatattacctatgcaaataaacatatattgttgaaaattgagaacctgatcgtgat
          tcttatgtgtaccatatgtatagtaatcacgcgactatatagtgctttagtatcgcccgt
          gggtgagtgaatattctgggctagcgtgagatagtttcttgtcctaatatttttcagatc
          gaatagcttctatttttgtgtttattgacatatgtcgaaactccttactcagtgaaagtc
          atgaccagatccacgaacaatcttcggaatcagtctcgttttacggcggaatcttgagtc
          taacttatatcccgtcgcttactttctaacaccccttatgtatttttaaaattacgttta
          ttcgaacgtacttggcggaagcgttattttttgaagtaagttacattgggcagactcttg
          acattttcgatacgactttctttcatccatcacaggactcgttcgtattgatatcagaag
          ctcgtgatgattagttgtcttctttaccaatactttgaggcctattctgcgaaatttttg
          ttgccctgcgaacttcacataccaaggaacacctcgcaacatgccttcatatccatcgtt
          cattgtaattcttacacaatgaatcctaagtaattacatccctgcgtaaaagatggtagg
          ggcactgaggatatattaccaagcatttagttatgagtaatcagcaatgtttcttgtatt
          aagttctctaaaatagttacatcgtaatgttatctcgggttccgcgaataaacgagatag
          attcattatatatggccctaagcaaaaacctcctcgtattctgttggtaattagaatcac
          acaatacgggttgagatattaattatttgtagtacgaagagatataaaaagatgaacaat
          tactcaagtcaagatgtatacgggatttataataaaaatcgggtagagatctgctttgca
          attcagacgtgccactaaatcgtaatatgtcgcgttacatcagaaagggtaactattatt
          aattaataaagggcttaatcactacatattagatcttatccgatagtcttatctattcgt
          tgtatttttaagcggttctaattcagtcattatatcagtgctccgagttctttattattg
          ttttaaggatgacaaaatgcctcttgttataacgctgggagaagcagactaagagtcgga
          gcagttggtagaatgaggctgcaaaagacggtctcgacgaatggacagactttactaaac
          caatgaaagacagaagtagagcaaagtctgaagtggtatcagcttaattatgacaaccct
          taatacttccctttcgccgaatactggcgtggaaaggttttaaaagtcgaagtagttaga
          ggcatctctcgctcataaataggtagactactcgcaatccaatgtgactatgtaatactg
          ggaacatcagtccgcgatgcagcgtgtttatcaaccgtccccactcgcctggggagacat
          gagaccacccccgtggggattattagtccgcagtaatcgactcttgacaatccttttcga
          ttatgtcatagcaatttacgacagttcagcgaagtgactactcggcgaaatggtattact
          aaagcattcgaacccacatgaatgtgattcttggcaatttctaatccactaaagcttttc
          cgttgaatctggttgtagatatttatataagttcactaattaagatcacggtagtatatt
          gatagtgatgtctttgcaagaggttggccgaggaatttacggattctctattgatacaat
          ttgtctggcttataactcttaaggctgaaccaggcgtttttagacgacttgatcagctgt
          tagaatggtttggactccctctttcatgtcagtaacatttcagccgttattgttacgata
          tgcttgaacaatattgatctaccacacacccatagtatattttataggtcatgctgttac
          ctacgagcatggtattccacttcccattcaatgagtattcaacatcactagcctcagaga
          tgatgacccacctctaataacgtcacgttgcggccatgtgaaacctgaacttgagtagac
          gatatcaagcgctttaaattgcatataacatttgagggtaaagctaagcggatgctttat
          ataatcaatactcaataataagatttgattgcattttagagttatgacacgacatagttc
          actaacgagttactattcccagatctagactgaagtactgatcgagacgatccttacgtc
          gatgatcgttagttatcgacttaggtcgggtctctagcggtattggtacttaaccggaca
          ctatactaataacccatgatcaaagcataacagaatacagacgataatttcgccaacata
          tatgtacagaccccaagcatgagaagctcattgaaagctatcattgaagtcccgctcaca
          atgtgtcttttccagacggtttaactggttcccgggagtcctggagtttcgacttacata
          aatggaaacaatgtattttgctaatttatctatagcgtcatttggaccaatacagaatat
          tatgttgcctagtaatccactataacccgcaagtgctgatagaaaatttttagacgattt
          ataaatgccccaagtatccctcccgtgaatcctccgttatactaattagtattcgttcat
          acgtataccgcgcatatatgaacatttggcgataaggcgcgtgaattgttacgtgacaga
          gatagcagtttcttgtgatatggttaacagacgtacatgaagggaaactttatatctata
          gtgatgcttccgtagaaataccgccactggtctgccaatgatgaagtatgtagctttagg
          tttgtactatgaggctttcgtttgtttgcagagtataacagttgcgagtgaaaaaccgac
          gaatttatactaatacgctttcactattggctacaaaatagggaagagtttcaatcatga
          gagggagtatatggatgctttgtagctaaaggtagaacgtatgtatatgctgccgttcat
          tcttgaaagatacataagcgataagttacgacaattataagcaacatccctaccttcgta
          acgatttcactgttactgcgcttgaaatacactatggggctattggcggagagaagcaga
          tcgcgccgagcatatacgagacctataatgttgatgatagagaaggcgtctgaattgata
          catcgaagtacactttctttcgtagtatctctcgtcctctttctatctccggacacaaga
          attaagttatatatatagagtcttaccaatcatgttgaatcctgattctcagagttcttt
          ggcgggccttgtgatgactgagaaacaatgcaatattgctccaaatttcctaagcaaatt
          ctcggttatgttatgttatcagcaaagcgttacgttatgttatttaaatctggaatgacg
          gagcgaagttcttatgtcggtgtgggaataattcttttgaagacagcactccttaaataa
          tatcgctccgtgtttgtatttatcgaatgggtctgtaaccttgcacaagcaaatcggtgg
          tgtatatatcggataacaattaatacgatgttcatagtgacagtatactgatcgagtcct
          ctaaagtcaattacctcacttaacaatctcattgatgttgtgtcattcccggtatcgccc
          gtagtatgtgctctgattgaccgagtgtgaaccaaggaacatctactaatgcctttgtta
          ggtaagatctctctgaattccttcgtgccaacttaaaacattatcaaaatttcttctact
          tggattaactacttttacgagcatggcaaattcccctgtggaagacggttcattattatc
          ggaaaccttatagaaattgcgtgttgactgaaattagatttttattgtaagagttgcatc
          tttgcgattcctctggtctagcttccaatgaacagtcctcccttctattcgacatcgggt
          ccttcgtacatgtctttgcgatgtaataattaggttcggagtgtggccttaatgggtgca
          actaggaatacaacgcaaatttgctgacatgatagcaaatcggtatgccggcaccaaaac
          gtgctccttgcttagcttgtgaatgagactcagtagttaaataaatccatatctgcaatc
          gattccacaggtattgtccactatctttgaactactctaagagatacaagcttagctgag
          accgaggtgtatatgactacgctgatatctgtaaggtaccaatgcaggcaaagtatgcga
          gaagctaataccggctgtttccagctttataagattaaaatttggctgtcctggcggcct
          cagaattgttctatcgtaatcagttggttcattaattagctaagtacgaggtacaactta
          tctgtcccagaacagctccacaagtttttttacagccgaaacccctgtgtgaatcttaat
          atccaagcgcgttatctgattagagtttacaactcagtattttatcagtacgttttgttt
          ccaacattacccggtatgacaaaatgacgccacgtgtcgaataatggtctgaccaatgta
          ggaagtgaaaagataaatat
          """.Replace("\r\n", "\n").Trim();
}
