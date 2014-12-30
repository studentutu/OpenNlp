﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLP.Tools.Util.Trees.TRegex.Tsurgeon
{
    /** Excises all nodes from the top to the bottom, and puts all the children of bottom node in where the top was.
 * @author Roger Levy (rog@stanford.edu)
 */
    public class ExciseNode:TsurgeonPattern
    {
        /**
   * Top should evaluate to a node that dominates bottom, but this is not checked!
   */
  public ExciseNode(TsurgeonPattern top, TsurgeonPattern bottom):
    base("excise", new TsurgeonPattern[] { top, bottom }){}

  /**
   * Excises only the directed node.
   */
  public ExciseNode(TsurgeonPattern node):
    base("excise", new TsurgeonPattern[] { node,node }){}

  //@Override
  public override TsurgeonMatcher matcher(Dictionary<String,Tree> newNodeNames, CoindexationGenerator coindexer) {
    return new Matcher(newNodeNames, coindexer, this);
  }

  private class Matcher : TsurgeonMatcher
  {
      private ExciseNode node;
    public Matcher(Dictionary<String,Tree> newNodeNames, CoindexationGenerator coindexer, ExciseNode node):
      base(node, newNodeNames, coindexer)
        {
            this.node = node;
        }

    //@Override
    public override Tree evaluate(Tree tree, TregexMatcher tregex) {
      Tree topNode = childMatcher[0].evaluate(tree, tregex);
      Tree bottomNode = childMatcher[1].evaluate(tree, tregex);
      /*if(Tsurgeon.verbose) {
        System.err.println("Excising...original tree:");
        tree.pennPrint(System.err);
        System.err.println("top: " + topNode + "\nbottom:" + bottomNode);
      }*/
      if (topNode == tree) {
        if (bottomNode.children().Length == 1) {
          return bottomNode.children()[0];
        } else {
          return null;
        }
      }
      Tree parent = topNode.parent(tree);
      /*if(Tsurgeon.verbose)
        System.err.println("Parent: " + parent);*/
      int i = Trees.objectEqualityIndexOf(parent,topNode);
      parent.removeChild(i);
      foreach(Tree child in bottomNode.children()) {
        parent.addChild(i,child);
        i++;
      }
      /*if(Tsurgeon.verbose)
        tree.pennPrint(System.err);*/
      return tree;
    }
  }
    }
}