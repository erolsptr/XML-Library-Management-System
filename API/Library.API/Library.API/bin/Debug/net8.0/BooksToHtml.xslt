<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" doctype-public="-//W3C//DTD HTML 4.01//EN" indent="yes"/>

  <!-- Kök element (/) eşleştiğinde bu şablon çalışır -->
  <xsl:template match="/">
    <html>
      <head>
        <title>Library Book List</title>
        <style>
          body { font-family: Arial, sans-serif; margin: 2em; }
          h1 { color: #333; }
          table { border-collapse: collapse; width: 80%; margin-top: 1em; }
          th, td { border: 1px solid #ccc; padding: 8px; text-align: left; }
          th { background-color: #f2f2f2; }
          tr:nth-child(even) { background-color: #f9f9f9; }
        </style>
      </head>
      <body>
        <h1>Book Catalog</h1>
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Title</th>
              <th>Author</th>
              <th>Publication Year</th>
              <th>Genre</th>
            </tr>
          </thead>
          <tbody>
            <!-- Library/Books/Book yolundaki her bir Book elementi için bu kısmı tekrarla -->
            <xsl:for-each select="Library/Books/Book">
              <!-- Kitapları Başlığa göre alfabetik olarak sırala -->
              <xsl:sort select="Title"/>
              <tr>
                <td><xsl:value-of select="@ID"/></td>
                <td><xsl:value-of select="Title"/></td>
                <td><xsl:value-of select="Author"/></td>
                <td><xsl:value-of select="PublicationYear"/></td>
                <td><xsl:value-of select="Genre"/></td>
              </tr>
            </xsl:for-each>
          </tbody>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>