using System;
using System.Collections.Generic;
using System.Linq;

var rawInput =
    "1531\n1959\n1344\n1508\n1275\n1729\n1904\n1740\n1977\n1992\n1821\n1647\n1404\n1893\n1576\n1509\n1995\n1637\n1816\n1884\n1608\n1943\n1825\n1902\n1227\n1214\n1675\n1650\n1752\n1818\n862\n2006\n227\n1504\n1724\n1961\n1758\n1803\n1991\n1126\n1909\n1643\n1980\n1889\n811\n1699\n1654\n1734\n1770\n1754\n1828\n1811\n1997\n1767\n1854\n1653\n1800\n1762\n1962\n1797\n877\n1660\n1895\n1939\n1679\n1496\n1606\n1262\n1727\n2005\n1796\n1595\n1846\n1822\n1974\n1829\n1347\n1341\n1875\n1726\n1963\n1659\n337\n1826\n1777\n1523\n1979\n1805\n1987\n2009\n1993\n374\n1546\n1706\n1748\n1743\n1725\n281\n1317\n1839\n1683\n1794\n1898\n1824\n1960\n1292\n1876\n1760\n1956\n1701\n1565\n1680\n1932\n1632\n1494\n1630\n1838\n1863\n1328\n1490\n1751\n1707\n1567\n1917\n1318\n1861\n519\n1716\n1891\n1636\n1684\n1200\n1933\n1911\n1809\n1967\n1731\n1921\n1827\n1663\n1720\n1976\n1236\n1986\n1942\n1656\n1733\n1541\n1640\n1518\n1897\n1676\n1307\n1978\n1998\n1674\n1817\n1845\n1658\n1639\n1842\n1929\n1972\n2010\n1951\n858\n1928\n1562\n1787\n1916\n1561\n1694\n1944\n1922\n1882\n1691\n589\n1940\n1624\n1570\n1557\n1791\n1492\n1919\n1615\n1571\n1657\n1984\n1521\n1766\n1790\n1782\n1874\n1198\n1764\n1278\n1688\n1905\n1786\n1281";
var input = rawInput.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

Console.WriteLine("Part 1");
var ints = ExpenseReport.FindSum(input, 2020);
Console.WriteLine(ints[0]);
Console.WriteLine(ints[1]);
Console.WriteLine(ints[0]*ints[1]);
Console.WriteLine();
Console.WriteLine("Part 1");
var findSum2 = ExpenseReport.FindSum2(input, 2020);
Console.WriteLine(findSum2[0]);
Console.WriteLine(findSum2[1]);
Console.WriteLine(findSum2[2]);
Console.WriteLine(findSum2[0]*findSum2[1]*findSum2[2]);

public static class ExpenseReport
{
    public static int[] FindSum(IReadOnlyCollection<int> rows, int sum)
    {
        var ordered = rows.OrderBy(x => x).ToArray();
        
        foreach (var a in ordered)
        {
            var diff = sum - a;
            var binarySearch = Array.BinarySearch(ordered, diff);
            if (binarySearch >= 0)
                return new[] {a, ordered[binarySearch]};
        }
        
        return Array.Empty<int>();
    }
    
    public static int[] FindSum2(int[] rows, int sum)
    {
        Array.Sort(rows);

        foreach (var a in rows)
        {
            var diffB = sum - a;
            foreach (var b in rows.TakeWhile(x => x <= diffB))
            {
                var diffC = diffB - b;
                var binarySearch = Array.BinarySearch(rows, diffC);
                if (binarySearch >= 0)
                    return new[] {a, b, rows[binarySearch]};    
            }
        }
        
        return Array.Empty<int>();
    }
}
