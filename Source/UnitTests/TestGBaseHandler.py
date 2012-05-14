#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Mon May  7 20:06:11 EDT 2012
# 
# 

import sys, unittest
sys.path.append('..')
import GBaseHandler
import GBaseExceptions
from os import system as s

class testGBaseHandler(unittest.TestCase):
	def setUp(self):
		self.beingTested = GBaseHandler.GBaseHandler()
		self.beingTested.GenGBase("foobar")
		self.beingTested.GenTable("spameggs")
		self.beingTested.GenColIn("id", "string", "spameggs")
		self.beingTested.GenColIn("spamname", "string", "spameggs")
		self.beingTested.GenRowIn(["id", "spamname"], ["1", "eggs"], "spameggs")
                self.beingTested.GenRowIn(["id", "spamname"], ["2", "Pizza"], "spameggs")
                self.beingTested.GenRowIn(["id", "spamname"], ["3", "wings"], "spameggs")
                self.beingTested.GenRowIn(["id", "spamname"], ["4", "fish"], "spameggs")
		self.beingTested.GenRowIn(["id", "spamname"], ['42', 'spammer1'], 'spameggs')
		self.beingTested.GenRowIn(["id", "spamname"], ['43', 'spammer2'], 'spameggs')
		self.beingTested.GenRowIn(["id", "spamname"], ['44', 'spammer3'], 'spameggs')
		self.beingTested.GenRowIn(["id", "spamname"], ['45', 'spammer1'], 'spameggs')

	def test_UseWorksProperlyIfCorrectGBaseIsGiven(self):
		self.beingTested.Use("FooBar")
		self.assertEqual("foobar", self.beingTested.use)

	def test_UseRaisesExceptionIfIncorrectGBaseIsGiven(self):
		self.assertRaises(GBaseExceptions.GBaseGeneralException, self.beingTested.Use, "FakeFakeFake")

	def test_GenGBaseMakesANiceFile(self):
		with open("./foobar.gbs", 'r') as readToTest:
			fileData = readToTest.read()
			self.assertEqual(fileData[:51], '<GBase Type="model"><HeaderInfo><Name>foobar</Name>')

	def test_GenTableMakesANiceFile(self):
		self.beingTested.GenTable("cars")
		with open("./cars-foobar.gbs", 'r') as readToTest:
                        fileData = readToTest.read()
                        self.assertEqual(fileData, '<GBase Type="table"><TableHeaderInfo><Name>cars</Name><Creator>michael</Creator></TableHeaderInfo><TableFields /><TableData /></GBase>')

	def test_GenColInWritesHeaderDataProperly(self):
		self.beingTested.GenColIn("testcol", "string", "spameggs")
		with open("./spameggs-foobar.gbs", 'r') as readToTest:
                        fileData = readToTest.read()
                        self.assertFalse(fileData.find('<Field Name="testcol"><Type>string</Type></Field>') == -1)

	def test_GenRowWithWritesANiceRow(self):
		self.beingTested.GenRowIn(["id", "spamname"], ["3", "eggs"], "spameggs")
		with open("./spameggs-foobar.gbs", 'r') as readToTest:
                        fileData = readToTest.read()
                        self.assertFalse(fileData.find('<Entity><D>3</D><D>eggs</D></Entity>') == -1)

	def test_multipleGetRowWithQueriesWork(self):
		result = self.beingTested.GetRowWith(["id"], ["2"], "spameggs")

		self.assertTrue(result[0][0] == "2" and result[0][1] == "Pizza")

		result = self.beingTested.GetRowWith([], [], "spameggs")

		self.assertTrue(result[0][0] == "1" and result[0][1] == "eggs")
		self.assertTrue(result[1][0] == "2" and result[1][1] == "Pizza")
		self.assertTrue(result[2][0] == "3" and result[2][1] == "wings")
		self.assertTrue(result[3][0] == "4" and result[3][1] == "fish")

		result = self.beingTested.GetRowWith(["id"], ["5"], "spameggs")

		self.assertTrue(result == [])

	def test_GetRowWithCanGetMultipleTuplesIfSpecifiedByTheWithAttributes(self):
		result = self.beingTested.GetRowWith(["spamname"], ["spammer1"], "spameggs")
		self.assertTrue(result[0][0] == "42" and result[0][1] == "spammer1")
                self.assertTrue(result[1][0] == "45" and result[1][1] == "spammer1")

		
	def test_DelGBaseGetsRidOfTheTableAndModelFiles(self):
		self.beingTested.DelGBase('foobar')
		self.assertRaises(IOError, open, 'foobar.gbs', 'r')
		self.assertRaises(IOError, open, 'foobar-testgbase.gbs', 'r')
		s("touch foobar")

	def test_DelTableGetsRidOfTheTableFile(self):
		self.beingTested.DelTable('spameggs')
		self.assertRaises(IOError, open, 'foobar-testgbase.gbs', 'r')

	def test_DelTableGetsRidOfDataInTheModelFile(self):
		self.beingTested.DelTable('spameggs')
		with open("./foobar.gbs", 'r') as readToTest:
                        fileData = readToTest.read()
                        self.assertTrue(fileData.find('Name="spameggs"') == -1)
			self.assertTrue(fileData.find('./spameggs-foobar.gbs') == -1)

	def test_DelRowRemovesTheProperData(self):
		self.beingTested.DelRowWith(['id'], ['42'], 'spameggs')
		with open("./spameggs-foobar.gbs", 'r') as readToTest:
                        fileData = readToTest.read()
			self.assertTrue(fileData.find('<Entry><D>42</D><D>spammer1</D></Entry>') == -1)

	def test_DelRowRemovesMultipleRowsIfWithConditionsSpecify(self):
		self.beingTested.DelRowWith(['spamname'], ['spammer1'], 'spameggs')
		with open("./spameggs-foobar.gbs", 'r') as readToTest:
                        fileData = readToTest.read()
                        self.assertTrue(fileData.find('<Entry><D>42</D><D>spammer1</D></Entry>') == -1)
			self.assertTrue(fileData.find('<Entry><D>45</D><D>spammer1</D></Entry>') == -1)

	def test_DelColDeletesTheProperDataFromTheFile(self):
		self.beingTested.DelColIn('id', 'spameggs')
		with open("./spameggs-foobar.gbs", 'r') as readToTest:
                        fileData = readToTest.read()
                        self.assertTrue(fileData.find('<D>1</D>') == -1)
			self.assertTrue(fileData.find('<D>2</D>') == -1)
			self.assertTrue(fileData.find('<D>3</D>') == -1)
			self.assertTrue(fileData.find('<D>4</D>') == -1)
			self.assertTrue(fileData.find('<D>42</D>') == -1)
			self.assertTrue(fileData.find('<D>43</D>') == -1)
			self.assertTrue(fileData.find('<D>44</D>') == -1)
			self.assertTrue(fileData.find('<Field Name="id"><Type>string</Type></Field>') == -1)
			self.assertFalse(fileData.find('spammer1') == -1)

	def test_ModRowWithMakesAChangeToTheFileCorrectly(self):
		self.beingTested.ModRowWith(["id"], ["42"], "spameggs", ["id"], ["35"])
		with open("./spameggs-foobar.gbs", 'r') as readToTest:
                        fileData = readToTest.read()
			self.assertTrue(fileData.find('<D>42</D>') == -1)
			self.assertFalse(fileData.find('<Entry><D>35</D><D>spammer1</D></Entry>') == -1)

	def test_ModRowWithWorksWhenMultipleRowsAreHitByTheGivenWithConditions(self):
		self.beingTested.ModRowWith(['spamname'], ['spammer1'], 'spameggs', ['spamname'], ['Mayhem'])
		with open("./spameggs-foobar.gbs", 'r') as readToTest:
                        fileData = readToTest.read()
                        self.assertFalse(fileData.find('<Entry><D>45</D><D>Mayhem</D></Entry>') == -1)
			self.assertFalse(fileData.find('<Entry><D>42</D><D>Mayhem</D></Entry>') == -1)

	def tearDown(self):
		s("rm *foobar*")

def suite():
	suite = unittest.TestSuite()
	suite.addTest(unittest.makeSuite(testGBaseHandler))
	return suite

if __name__ == '__main__':
	unittest.TextTestRunner(verbosity=2).run(suite())
