#!/usr/bin/env python

# Created by Michael Gilliland
# Date: Sun May  6 20:26:50 EDT 2012
# 
# 

import xml.parsers.expat

test = {}
ends = []

# 3 handler functions
def start_element(name, attrs):
    print 'Start element:', name, attrs
    if name == "Creator":
	test["Creator"] = []
def end_element(name):
    print 'End element:', name
    ends.append(name)
def char_data(data):
    if data != u'\n' and data.replace(" ", '') != '':
        if "Creator" in test and "Creator" not in ends:
            test["Creator"].append(repr(data))

p = xml.parsers.expat.ParserCreate()

p.StartElementHandler = start_element
p.EndElementHandler = end_element
p.CharacterDataHandler = char_data

p.Parse(open("./testgbase.gbs", "r").read(), 1)

print test["Creator"]
