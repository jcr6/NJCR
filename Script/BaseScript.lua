-- <License Block>
-- Script/BaseScript.lua
-- Base Script for NJCR scripting engine
-- version: 20.11.10
-- Copyright (C) 2019, 2020 Jeroen P. Broks
-- This software is provided 'as-is', without any express or implied
-- warranty.  In no event will the authors be held liable for any damages
-- arising from the use of this software.
-- Permission is granted to anyone to use this software for any purpose,
-- including commercial applications, and to alter it and redistribute it
-- freely, subject to the following restrictions:
-- 1. The origin of this software must not be misrepresented; you must not
-- claim that you wrote the original software. If you use this software
-- in a product, an acknowledgment in the product documentation would be
-- appreciated but is not required.
-- 2. Altered source versions must be plainly marked as such, and must not be
-- misrepresented as being the original software.
-- 3. This notice may not be removed or altered from any source distribution.
-- </License Block>

local NJCR = __NJCR __NJCR = nil -- Security measure!
local trueJLS = {}
local metaJLS = {}

function metaJLS.__index(s,key)
	assert(trueJLS[key],"There is no property/method/field called "..key.." in JLS");
	return trueJLS[key]
end

function metaJLS.__newindex(s,key,value)
	assert(trueJLS["Accept"..key],"Can't write to key '"..key.."'. Either it doesn't exist, or is read-only!")
	trueJLS["Accept"..key](value)
end

function trueJLS.SetJCR6OutputFile(value)
	NJCR.OutputFile = tostring(value)
end

trueJLS.AcceptSetJCR6OutputFile = trueJLS.Output

function trueJLS.Output(value)
	NJCR:Output(value)
end

function trueJLS.Add(source,target,data)
	NJCR:SetAdd("Start","Let's go!") -- The 2nd parameter just had to be set, but has no meaning, whatsover!
	NJCR:SetAdd("Source",source)
	NJCR:SetAdd("Target",target)
	for k,v in pairs(data) do
		-- Please note that the system will simply ignore fields it doesn't know. This to prevent crashes with newer versions where more support is given (which is not deemed likely at the present time)
		NJCR:SetAdd(k,v)
	end
	NJCR:SetAdd("Close","Goodbye")
end

function trueJLS.Alias(ori,target)
	NJCR:AddAlias(ori,target)
end

function trueJLS.AcceptVerbose(v)
	NJCR.Verbose = v~=nil and v~=false
	trueJLS.Verbose = NJCR.Verbose
end
trueJLS.Verbose = NJCR.Verbose

function trueJLS.AddComment(n,c)
	assert(type(n)=="string","Comment name error!")
	assert(type(c)=="string","Comment content error!")
	NJCR:AddComment(n,c)
end

function trueJLS.GetDir(s)
	local s = NJCR:GetDir(s)
	local f = load("return {"..s.."}","GetDir Query")
	assert(f,"Get Dir not wll received!\n\n"..s)
	local ret = f()
	assert(ret,"Get Dir ould not generate correct output\n\n"..s)
	return ret
end


JLS = setmetatable({},metaJLS)

Add = JLS.Add
Alias = JLS.Alias
AddComment = JLS.AddComment
GetDir = JLS.GetDir