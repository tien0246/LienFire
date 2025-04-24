using System.Collections.Generic;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.Xslt;

internal class Stylesheet : StylesheetLevel
{
	private Compiler compiler;

	public List<Uri> ImportHrefs = new List<Uri>();

	public List<XslNode> GlobalVarPars = new List<XslNode>();

	public Dictionary<QilName, AttributeSet> AttributeSets = new Dictionary<QilName, AttributeSet>();

	private int importPrecedence;

	private int orderNumber;

	public List<WhitespaceRule>[] WhitespaceRules = new List<WhitespaceRule>[3];

	public List<Template> Templates = new List<Template>();

	public Dictionary<QilName, List<TemplateMatch>> TemplateMatches = new Dictionary<QilName, List<TemplateMatch>>();

	public int ImportPrecedence => importPrecedence;

	public void AddTemplateMatch(Template template, QilLoop filter)
	{
		if (!TemplateMatches.TryGetValue(template.Mode, out var value))
		{
			List<TemplateMatch> list = (TemplateMatches[template.Mode] = new List<TemplateMatch>());
			value = list;
		}
		value.Add(new TemplateMatch(template, filter));
	}

	public void SortTemplateMatches()
	{
		foreach (QilName key in TemplateMatches.Keys)
		{
			TemplateMatches[key].Sort(TemplateMatch.Comparer);
		}
	}

	public Stylesheet(Compiler compiler, int importPrecedence)
	{
		this.compiler = compiler;
		this.importPrecedence = importPrecedence;
		WhitespaceRules[0] = new List<WhitespaceRule>();
		WhitespaceRules[1] = new List<WhitespaceRule>();
		WhitespaceRules[2] = new List<WhitespaceRule>();
	}

	public void AddWhitespaceRule(int index, WhitespaceRule rule)
	{
		WhitespaceRules[index].Add(rule);
	}

	public bool AddVarPar(VarPar var)
	{
		foreach (XslNode globalVarPar in GlobalVarPars)
		{
			if (globalVarPar.Name.Equals(var.Name))
			{
				return compiler.AllGlobalVarPars.ContainsKey(var.Name);
			}
		}
		GlobalVarPars.Add(var);
		return true;
	}

	public bool AddTemplate(Template template)
	{
		template.ImportPrecedence = importPrecedence;
		template.OrderNumber = orderNumber++;
		compiler.AllTemplates.Add(template);
		if (template.Name != null)
		{
			if (!compiler.NamedTemplates.TryGetValue(template.Name, out var value))
			{
				compiler.NamedTemplates[template.Name] = template;
			}
			else if (value.ImportPrecedence == template.ImportPrecedence)
			{
				return false;
			}
		}
		if (template.Match != null)
		{
			Templates.Add(template);
		}
		return true;
	}
}
