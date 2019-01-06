<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="xml" indent="yes"/>
  <xsl:param name ="id">
  </xsl:param>
    <xsl:template match="/*">
        <xsl:copy>
          <xsl:for-each select="*">
            <xsl:if test="not (@id = $id) and not (@channel = $id)">
              <xsl:copy-of select="."/>
            </xsl:if>
          </xsl:for-each>
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>
