#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Sun May  6 20:12:34 EDT 2012
# 
# 

import GBaseBrain

class GBaseHandler:
	def __init__(self):
		self.use = ""
		self.brain = GBaseBrain.GBaseBrain()

	def Use(self, gbaseName):
		if self.brain.FileIsModel(self.brain.GetModelFileName(gbaseName)):
			self.use = gbaseName.lower()
