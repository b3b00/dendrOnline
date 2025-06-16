import { gfmToMarkdown } from "mdast-util-gfm";
import { visit } from "unist-util-visit";
import sha1 from "sync-sha1";

import remarkGfm from "remark-gfm";
import remarkParse from "remark-parse";
import { unified } from "unified";
import { toMarkdown } from "mdast-util-to-markdown";

let hashItem = (message: string): string => sha1(message).toString("hex");

function Toggle(item: string) {
  return function togglingTransformer() {
    return function(tree) {
      const hash = hashItem(item);
      visit(tree, "listItem", (node) => {
        let thisItem = node.children[0].children[0].value;
        let md = toMarkdown(node);
        md = md.startsWith("* ") ? md.substring(2) : md;
        md = md.endsWith("\n") ? md.substring(0, md.length - 1) : md;
        let thisHash = hashItem(thisItem);
        let thisMdHash = hashItem(md);
        if (hash === null || hash == thisMdHash) {
          node.children[0].children[0].value =
            node.children[0].children[0].value;
          if (node.checked) {
            node.checked = false;
          } else {
            node.checked = true;
          }
          return node;
        }
      });
    };
  };
}

export const TaskToggler = {
  hashItem: hashItem,

  Toggle: async (
    item: string,
    itemHash: string,
    md: string
  ): Promise<string> => {
    const ast = unified()
      .use(remarkParse)
      .use(remarkGfm)
      .use(Toggle(item))
      .parse(md);
    const transformedAst = await unified()
      .use(Toggle(item))
      .run(ast);
    const result = toMarkdown(ast, {
      extensions: [gfmToMarkdown()],
    });

    return result;
  },
};
